using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class EmissiveImageConfigurator : MonoBehaviour
{
    [SerializeField] private Material emissiveMat;
    [SerializeField] private bool emitOnStart = false;
    [SerializeField] private bool useImageColor = false;
    [SerializeField] private OverrideType colorOverride = OverrideType.NoOverride;
    private Color prevImgColor;
    private Image image;
    private bool activated = false;
    private float initIntensity;
    bool materialCoppied = false;

    private void Awake()
    {
        emissiveMat = new(emissiveMat);
        materialCoppied = true;
        initIntensity = emissiveMat.GetFloat("_CurrentForce");
        image = GetComponent<Image>();
        ToggleEmission(emitOnStart);
    }

    public void ToggleEmission(bool activate, bool forceUpdate = false)
    {
        if (activated == activate && !forceUpdate) return;
        image.material = activate ? emissiveMat : image.defaultMaterial;
        if (activate && useImageColor) SetColor(image.color);
        CheckColorOverride(activate);
        activated = !activated;
    }

    public void SetIntensityPercentage(float intensity)
    {
        if (!materialCoppied) return;
        intensity = Mathf.Clamp01(intensity);
        emissiveMat.SetFloat("_CurrentForce", initIntensity * intensity);
    }

    public void SetColor(Color color)
    {
        if (!materialCoppied) return;
        emissiveMat.SetColor("_EmissionColor", color);
    }

    private void CheckColorOverride(bool activate)
    {
        if (colorOverride == OverrideType.NoOverride) return;
        if (activate)
        {
            prevImgColor = image.color;
            image.color = Color.white;
        }
        else if (image.color == Color.white && colorOverride != OverrideType.OnlySet) 
            image.color = prevImgColor;
    }

    private enum OverrideType
    {
        NoOverride,
        SetAndRestore,
        OnlySet
    }
}
