using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NavigationButton : MonoBehaviour
{
    [SerializeField] private Screens destination;
    private NavigationController controller;

    private void Awake()
    {
        controller = GetComponentInParent<NavigationController>();
        if (!controller) Debug.LogError("Error: NavigationButton fuera de la jerarquía del controller.");
        GetComponent<Button>().onClick.AddListener(Navigate);
    }

    public void Navigate() => controller.GoTo(destination);
}
