using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class EmissiveImageConfigurator : MonoBehaviour
{
    [SerializeField] private Material emissiveMat;
    private Image image;
    private bool activated = false;

    private void Awake()
    {
        emissiveMat = new(emissiveMat);
        image = GetComponent<Image>();
    }

    public void ToggleEmission(bool activate)
    {
        if (activated == activate) return;
        image.material = activate ? emissiveMat : image.defaultMaterial;
        activated = !activated;
    }
}
