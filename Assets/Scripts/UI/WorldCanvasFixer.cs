using UnityEngine;

/// <summary>
/// Con los canvas que necesitan ser vistos por la cámara de UI y estar en screen space camera no se puede interactuar
/// Para ello, se simula de forma simple el espacio de cámara estando en world space.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class WorldCanvasFixer : MonoBehaviour
{
    [SerializeField] private float distanceToCam;

    void Start()
    {
        TryGetComponent(out Canvas canvas);
        Camera cam = GameObject.FindWithTag("UICam").GetComponent<Camera>();
        bool prevCanvasState = canvas.enabled;
        canvas.enabled = true; //Si está desactivado no funciona (gracias Unity)
        canvas.worldCamera = cam; //UICam mantiene fov, así que se configura con ella
        canvas.renderMode = RenderMode.ScreenSpaceCamera; //Se pone en la posición deseada automáticamente
        canvas.planeDistance = distanceToCam; //Distancia deseada
        Canvas.ForceUpdateCanvases();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main; //Para que se pueda interactuar
        canvas.transform.parent = Camera.main.transform; //Para que se mueva con la cámara
        canvas.enabled = prevCanvasState;
    }
}
