using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class NavigationObject : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Screens destination;
    private NavigationController controller;

    private void Awake()
    {
        controller = FindFirstObjectByType<NavigationController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        controller.GoTo(destination, this.gameObject);
    }
}
