using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class LoreMenuTransitionManager : MonoBehaviour, INavigationCtxReceiver, INavigationLeaveReceiver
{
    private RawImage vignette;
    private float initVignetteIntensity;
    private float initVignetteSmoothness;
    private CameraNavigation cameraNavigation;
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
        if(!GameObject.FindWithTag("Vignette").TryGetComponent(out vignette))
            Debug.LogError("Error: no hay Volume con viñeta");
        initVignetteIntensity = Intensity;
        initVignetteSmoothness = Smoothness;
        cameraNavigation = FindFirstObjectByType<CameraNavigation>();
        if (!cameraNavigation) Debug.LogError("Error: no se encuentra el sistema de navegación de cámara");
    }

    private void OnDestroy()
    {
        Intensity = initVignetteIntensity;
        Smoothness = initVignetteSmoothness;
    }

    public void ReceiveContext(Screens prevScreen, bool isGoingBack, GameObject sender = null)
    {
        Intensity = 0;
        Smoothness = 0;
        vignette.enabled = true;
        StopAllCoroutines();
        StartCoroutine(VignetteTransitionCoroutine(initVignetteIntensity, initVignetteSmoothness));
    }

    public void OnLeave()
    {
        StopAllCoroutines();
        StartCoroutine(VignetteTransitionCoroutine(0, 0));
    }

    IEnumerator VignetteTransitionCoroutine(float targetIntensity, float targetSmoothness)
    {
        float intensitySpeed = initVignetteIntensity / cameraNavigation.InterpolationTime;
        float smoothnessSpeed = initVignetteSmoothness / cameraNavigation.InterpolationTime;
        while(Intensity != targetIntensity && Smoothness != targetSmoothness)
        {
            Intensity = Mathf.MoveTowards(Intensity, targetIntensity, intensitySpeed * Time.deltaTime);
            Smoothness = Mathf.MoveTowards(Smoothness, targetSmoothness, smoothnessSpeed * Time.deltaTime);
            yield return null;
        }
        if (targetIntensity == 0) vignette.enabled = false;
    }
}
