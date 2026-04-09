using System.Collections;
using UnityEngine;

public class CameraNavigation : MonoBehaviour
{
    [SerializeField] private float interpolationTime = 1f;
    private Transform cam;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    public void MoveTo(Transform dest)
    {
        StopAllCoroutines();
        StartCoroutine(MovementCoroutine(dest));
    }

    private IEnumerator MovementCoroutine(Transform dest)
    {
        float moveSpeed = Vector3.Distance(dest.position, cam.position) / interpolationTime;
        float rotSpeed = Quaternion.Angle(dest.rotation, cam.rotation) / interpolationTime;
        while (cam.position != dest.position || cam.rotation != dest.rotation)
        {
            cam.position = Vector3.MoveTowards(cam.position, dest.position, moveSpeed * Time.deltaTime);
            cam.rotation = Quaternion.RotateTowards(cam.rotation, dest.rotation, rotSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
