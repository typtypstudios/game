using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class NavigationObject : MonoBehaviour, IPointerClickHandler, ICursorHoverTarget
{
    [SerializeField] private Screens destination;
    [SerializeField] private float minInteractionDistance = 20;
    private NavigationController controller;
    private bool isFirstScreenLoaded = false;

    private void Awake()
    {
        controller = FindFirstObjectByType<NavigationController>();
        CanvasTransitionManager.OnTransitionFinished += OnFirstSceneLoaded;
    }

    private void OnFirstSceneLoaded()
    {
        isFirstScreenLoaded = true;
        CanvasTransitionManager.OnTransitionFinished -= OnFirstSceneLoaded;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CanBeInteracted()) return;
        controller.GoTo(destination, this.gameObject);
    }

    public bool CanBeInteracted()
    {
        return isFirstScreenLoaded && 
            Vector3.Distance(transform.position, Camera.main.transform.position) >= minInteractionDistance;
    }

    public bool CanCauseHover()
    {
        return CanBeInteracted();
    }
}
