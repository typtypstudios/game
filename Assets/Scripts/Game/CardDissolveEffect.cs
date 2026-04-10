using System;
using System.Collections;
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

    public void SetDissolve(float dissolve) => Dissolve = Mathf.Clamp01(dissolve);

    public void FadeInAndOut(float transitionTime, float showTime, Action onStart)
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(transitionTime, showTime, onStart));
    }
        
    IEnumerator FadeCoroutine(float transitionTime, float showTime, Action onStart)
    {
        float speed = 1 / transitionTime;
        float dissolve = Dissolve;
        while (dissolve < 1) //Por si había una carta ya enseñándose
        {
            dissolve += speed * Time.deltaTime;
            Dissolve = dissolve;
            yield return null;
        }
        onStart?.Invoke();
        dissolve = 1;
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
    }
}
