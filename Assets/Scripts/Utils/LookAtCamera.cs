using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool invert;
    private Transform cam;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    private void Update()
    {
        transform.forward = (invert ? -1 : 1) * transform.position - cam.position;
    }
}
