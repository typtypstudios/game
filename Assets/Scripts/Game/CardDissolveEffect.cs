using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CardDissolveEffect : MonoBehaviour
{
    [SerializeField] private Image[] linkedImages;
    [SerializeField] private Material dissolveMat;
    private float Dissolve
    {
        get { return dissolveMat.GetFloat("_Dissolve"); }
        set { dissolveMat.SetFloat("_Dissolve", value); }
    }

    private void Awake()
    {
        dissolveMat = new(dissolveMat);
        GetComponent<Image>().material = dissolveMat;
        foreach (var image in linkedImages) image.material = dissolveMat;
        Dissolve = 1;
    }

    public void SetDissolve(float dissolve, bool interpolate = false, float interpolateTime = 1)
    {
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
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(transitionTime, showTime, onStart, onEnd, dissolvePrevContent));
    }
        
    IEnumerator FadeCoroutine(float transitionTime, float showTime, Action onStart,
        Action onEnd, bool initialDissolve)
    {
        float speed = 1 / transitionTime;
        float dissolve = Dissolve;
        if (initialDissolve)
        {
            while (dissolve < 1) //Por si había una carta ya enseñándose
            {
                dissolve += speed * Time.deltaTime;
                Dissolve = dissolve;
                yield return null;
            }
            dissolve = 1;
        }
        onStart?.Invoke();
        while(dissolve > 0)
        {
            dissolve -= speed * Time.deltaTime;
            Dissolve = dissolve;
            yield return null;
        }
        yield return new WaitForSeconds(showTime);
        dissolve = 0;
        while(dissolve < 1)
        {
            dissolve += speed * Time.deltaTime;
            Dissolve = dissolve;
            yield return null;
        }
        onEnd?.Invoke();
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
