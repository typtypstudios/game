using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class LoreMenu : MonoBehaviour, INavigationCtxReceiver, INavigationLeaveReceiver
{
    [SerializeField] private GameObject skullEyes;
    private CameraNavigation cameraNavigation;
    private VignetteController vignette;
    private bool onFocus = false;

    private void Awake()
    {
        cameraNavigation = FindFirstObjectByType<CameraNavigation>();
        if (!cameraNavigation) Debug.LogError("Error: no se encuentra el sistema de navegación de cámara");
        vignette = FindFirstObjectByType<VignetteController>();
        if (!vignette) Debug.LogWarning("Aviso: no se encuentra la viñeta");
        CanvasTransitionManager.OnDissolved += ToggleEyes;
        ToggleEyes();
    }

    private void OnDestroy() => CanvasTransitionManager.OnDissolved -= ToggleEyes;

    private void ToggleEyes()
    {
        skullEyes.SetActive(onFocus);
    }

    public void ReceiveContext(Screens prevScreen, bool isGoingBack, GameObject sender = null)
    {
        onFocus = true;
        vignette.FadeIn(cameraNavigation.InterpolationTime);
    }
        
    public void OnLeave()
    {
        onFocus = false;
        vignette.FadeOut(cameraNavigation.InterpolationTime);
    }
}