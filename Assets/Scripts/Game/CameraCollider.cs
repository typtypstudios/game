using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraCollider : MonoBehaviour
{
    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground")) 
            cam.cullingMask = 1 << LayerMask.NameToLayer("TransitionCanvas");
    }
}
