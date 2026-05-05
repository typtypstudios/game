using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class NavigationObject : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Screens destination;
    [SerializeField] private float minInteractionDistance = 20;
    private NavigationController controller;

    private void Awake()
    {
        controller = FindFirstObjectByType<NavigationController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Vector3.Distance(transform.position, Camera.main.transform.position) < minInteractionDistance) return;
        controller.GoTo(destination, this.gameObject);
    }
}
