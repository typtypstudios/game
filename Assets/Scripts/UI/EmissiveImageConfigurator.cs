using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class EmissiveImageConfigurator : MonoBehaviour
{
    [SerializeField] private Material emissiveMat;
    [SerializeField] private bool emitOnStart = false;
    private Image image;
    private bool activated = false;
    private float initIntensity;
    bool materialCoppied = false;

    private void Awake()
    {
        emissiveMat = new(emissiveMat);
        materialCoppied = true;
        initIntensity = emissiveMat.GetFloat("_EmissionForce");
        image = GetComponent<Image>();
        ToggleEmission(emitOnStart);
    }

    public void ToggleEmission(bool activate)
    {
        if (activated == activate) return;
        image.material = activate ? emissiveMat : image.defaultMaterial;
        activated = !activated;
    }

    public void SetIntensityPercentage(float intensity)
    {
        if (!materialCoppied) return;
        intensity = Mathf.Clamp01(intensity);
        emissiveMat.SetFloat("_EmissionForce", initIntensity * intensity);
    }

    public void SetTint(Color tint, float intensity)
    {
        if (!materialCoppied) return;
        emissiveMat.SetColor("_Tint", Utils.ColorToHDR(tint, intensity));
    }
}
