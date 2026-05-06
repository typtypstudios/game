using System.Collections;
using UnityEngine;

public class EyeMovement : MonoBehaviour
{
    [SerializeField] private float maxAngle = 5;
    [SerializeField] private float movementSpeed = 1;
    [SerializeField] private Vector2 targetChangeInterval = new(0.5f, 1.5f);
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    private void Awake()
    {
        initialRotation = transform.rotation;
    }

    private void OnEnable()
    {
        StartCoroutine(ChangeTargetCoroutine());
    }

    private void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, 
            targetRotation, movementSpeed * Time.deltaTime);
    }

    IEnumerator ChangeTargetCoroutine()
    {
        while (true)
        {
            Vector3 axis = Vector3.Normalize(new(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
            float angle = Random.Range(-maxAngle, maxAngle);
            targetRotation = initialRotation * Quaternion.AngleAxis(angle, axis);
            yield return new WaitForSeconds(Random.Range(targetChangeInterval.x, targetChangeInterval.y));
        }
    }
}
