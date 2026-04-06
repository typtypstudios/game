using UnityEngine;

public class NavigationButton : MonoBehaviour
{
    [SerializeField] private Screens destination;
    private NavigationController controller;

    private void Awake()
    {
        controller = GetComponentInParent<NavigationController>();
        if (!controller) Debug.LogError("Error: NavigationButton fuera de la jerarquía del controller.");
    }

    public void Navigate() => controller.GoTo(destination);

    public void GoBack() => controller.GoBack();
}
