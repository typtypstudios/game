using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Canvas))]
public class LoreMenu : MonoBehaviour, INavigationCtxReceiver, INavigationLeaveReceiver
{
    [Range(0, 1)][SerializeField] private float vignetteIntensity = 0.5f;
    private Vignette vignette;
    private float initVignetteIntensity;
    private CameraNavigation cameraNavigation;

    private void Awake()
    {
        if(!FindObjectsByType<Volume>(FindObjectsSortMode.None).
            Where(v => v.isGlobal).
            First().
            profile.TryGet(out vignette)) Debug.LogError("Error: no hay Volume con viñeta");
        vignette.intensity.overrideState = true;
        cameraNavigation = FindFirstObjectByType<CameraNavigation>();
        if (!cameraNavigation) Debug.LogError("Error: no se encuentra el sistema de navegación de cámara");
        initVignetteIntensity = vignette.intensity.value;
    }

    private void OnDestroy()
    {
        vignette.intensity.value = initVignetteIntensity;
    }

    public void ReceiveContext(Screens prevScreen, bool isGoingBack, GameObject sender = null)
    {
        StopAllCoroutines();
        StartCoroutine(VignetteTransitionCoroutine(vignetteIntensity));
    }

    public void OnLeave()
    {
        StopAllCoroutines();
        StartCoroutine(VignetteTransitionCoroutine(0));
    }

    IEnumerator VignetteTransitionCoroutine(float targetIntensity)
    {
        float speed = initVignetteIntensity / cameraNavigation.InterpolationTime;
        while(vignette.intensity.value != targetIntensity)
        {
            vignette.intensity.value = Mathf.MoveTowards(vignette.intensity.value, 
                targetIntensity, speed * Time.deltaTime);
            yield return null;
        }
        Debug.Log(vignette.intensity.value);
    }
}
