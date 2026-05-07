using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CardDissolveEffect : MonoBehaviour
{
    [SerializeField] private Image[] linkedImages;
    [SerializeField] private Transform[] linkedImageContainers;
    [SerializeField] private Material dissolveMat;
    private Material sourceMat;

    private float Dissolve
    {
        get { return dissolveMat.GetFloat("_Dissolve"); }
        set { dissolveMat.SetFloat("_Dissolve", value); }
    }

    private void Awake()
    {
        sourceMat = dissolveMat;
        dissolveMat = new(dissolveMat);
        UpdateMaterials();
        Dissolve = 1;
    }

    public void SetDissolve(float dissolve, bool interpolate = false, float interpolateTime = 1)
    {
        UpdateMaterials();
        dissolve = Mathf.Clamp01(dissolve);
        if (Dissolve == dissolve) return;
        if (!interpolate) Dissolve = dissolve;
        else
        {
            StopAllCoroutines();
            StartCoroutine(InterpolateToValue(dissolve, interpolateTime));
        }
    }

    public void FadeInAndOut(float transitionTime, float showTime, Action onStart, Action onEnd,
        bool dissolvePrevContent = true)
    {
        UpdateMaterials();
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(transitionTime, showTime, onStart, onEnd, dissolvePrevContent));
    }

    /// <summary>
    /// El contenido desaparece y aparece de vuelta
    /// </summary>
    /// <param name="blinkTime"></param>
    /// <param name="onBlink"></param>
    public void Blink(float blinkTime, Action onBlink)
    {
        UpdateMaterials();
        StopAllCoroutines();
        StartCoroutine(BlinkCoroutine(blinkTime, onBlink));
    }

    public void OverrideMaterial(Material mat, float startingDissolve)
    {
        if (mat == sourceMat) return;
        sourceMat = mat;
        dissolveMat = new(mat);
        UpdateMaterials();
        Dissolve = startingDissolve;
    }

    private void UpdateMaterials()
    {
        GetComponent<Image>().material = dissolveMat;

        foreach (var image in linkedImages)
        {
            if (image == null) continue;
            image.material = dissolveMat;
        }

        foreach (var container in linkedImageContainers)
        {
            if (container == null) continue;
            foreach (var image in container.GetComponentsInChildren<Image>(true))
            {
                image.material = dissolveMat;
            }
        }
    }

    IEnumerator FadeCoroutine(float transitionTime, float showTime, Action onStart,
        Action onEnd, bool initialDissolve)
    {
        float speed = 1 / transitionTime;
        float dissolve = Dissolve;
        if (initialDissolve)
        {
            while (dissolve < 1) //Por si habia una carta ya ensenandose
            {
                dissolve += speed * Time.deltaTime;
                Dissolve = dissolve;
                yield return null;
            }
            dissolve = 1;
        }

        onStart?.Invoke();
        while (dissolve > 0)
        {
            dissolve -= speed * Time.deltaTime;
            Dissolve = dissolve;
            yield return null;
        }

        yield return new WaitForSeconds(showTime);
        AudioManager.Instance.PlayUI(UISound.DissolveOut);
        dissolve = 0;
        while (dissolve < 1)
        {
            dissolve += speed * Time.deltaTime;
            Dissolve = dissolve;
            yield return null;
        }

        onEnd?.Invoke();
    }

    IEnumerator BlinkCoroutine(float blinkTime, Action onBlink)
    {
        float speed = 2 / blinkTime;
        float dissolve = Dissolve;
        while (dissolve < 1)
        {
            dissolve += speed * Time.deltaTime;
            Dissolve = dissolve;
            yield return null;
        }
        dissolve = 1;
        onBlink?.Invoke();
        while (dissolve > 0)
        {
            dissolve -= speed * Time.deltaTime;
            Dissolve = dissolve;
            yield return null;
        }
    }

    IEnumerator InterpolateToValue(float targetDissolve, float time)
    {
        float dissolve = Dissolve;
        float speed = Mathf.Abs(targetDissolve - dissolve) / time;
        while (dissolve != targetDissolve)
        {
            dissolve = Mathf.MoveTowards(dissolve, targetDissolve, speed * Time.deltaTime);
            Dissolve = dissolve;
            yield return null;
        }
    }
}
