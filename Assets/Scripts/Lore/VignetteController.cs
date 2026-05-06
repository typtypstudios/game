using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class VignetteController : MonoBehaviour
{
    private RawImage vignette;
    private float initVignetteIntensity;
    private float initVignetteSmoothness;
    private float Smoothness
    {
        get { return vignette.material.GetFloat("_Smoothness"); }
        set { vignette.material.SetFloat("_Smoothness", value); }
    }
    private float Intensity
    {
        get { return vignette.material.GetFloat("_Intensity"); }
        set { vignette.material.SetFloat("_Intensity", value); }
    }

    private void Awake()
    {
        vignette = GetComponent<RawImage>();
        initVignetteIntensity = Intensity;
        initVignetteSmoothness = Smoothness;
    }

    private void OnDestroy()
    {
        Intensity = initVignetteIntensity;
        Smoothness = initVignetteSmoothness;
    }

    public void FadeIn(float time)
    {
        Intensity = 0;
        Smoothness = 0;
        vignette.enabled = true;
        StopAllCoroutines();
        StartCoroutine(VignetteTransitionCoroutine(initVignetteIntensity, initVignetteSmoothness, time));
    }

    public void FadeOut(float time)
    {
        StopAllCoroutines();
        StartCoroutine(VignetteTransitionCoroutine(0, 0, time));
    }

    IEnumerator VignetteTransitionCoroutine(float targetIntensity, float targetSmoothness, float time)
    {
        float intensitySpeed = initVignetteIntensity / time;
        float smoothnessSpeed = initVignetteSmoothness / time;
        while (Intensity != targetIntensity && Smoothness != targetSmoothness)
        {
            Intensity = Mathf.MoveTowards(Intensity, targetIntensity, intensitySpeed * Time.deltaTime);
            Smoothness = Mathf.MoveTowards(Smoothness, targetSmoothness, smoothnessSpeed * Time.deltaTime);
            yield return null;
        }
        if (targetIntensity == 0) vignette.enabled = false;
    }
}