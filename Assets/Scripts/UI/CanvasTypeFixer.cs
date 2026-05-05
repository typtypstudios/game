using UnityEngine;

/// <summary>
/// Con los canvas que necesitan ser vistos por la cámara de UI y estar en screen space camera no se puede interactuar
/// Para ello, se simula de forma simple el espacio de cámara estando en world space.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class CanvasTypeFixer : MonoBehaviour
{
    [SerializeField] private RenderMode initType;
    [SerializeField] private bool fixOnStart = true;
    [SerializeField] private float distanceToCam;
    private Canvas canvas;
    private Camera uiCam;

    void Start()
    {
        TryGetComponent(out canvas);
        if(fixOnStart) SetCanvasType(initType);
    }

    public void SetCanvasType(RenderMode type)
    {
        uiCam = GameObject.FindWithTag("UICam").GetComponent<Camera>();
        bool prevCanvasState = canvas.enabled;
        canvas.enabled = true; //Si está desactivado no funciona (gracias Unity)
        if (type == RenderMode.WorldSpace) FixToWorld();
        else if (type == RenderMode.ScreenSpaceCamera) FixToCamera();
        else FixToOverlay();
        canvas.enabled = prevCanvasState;
    }

    private void FixToWorld()
    {
        FixToCamera(); //Unity ya lo pone automáticamente delante de la cámara, bien ajustado
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main; //Para que se pueda interactuar
        canvas.transform.SetParent(Camera.main.transform); //Para que se mueva con la cámara
    }

    private void FixToCamera()
    {
        canvas.worldCamera = uiCam; //UICam mantiene fov, así que se configura con ella
        canvas.renderMode = RenderMode.ScreenSpaceCamera; //Se pone en la posición deseada automáticamente
        canvas.planeDistance = distanceToCam; //Distancia deseada
        Canvas.ForceUpdateCanvases();
    }

    private void FixToOverlay()
    {
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }
}