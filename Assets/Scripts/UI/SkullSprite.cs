using TypTyp;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SkullSprite : MonoBehaviour
{
    [SerializeField] private EmissiveImageConfigurator configurator;
    [SerializeField] private bool overrideEmissionColor = true;
    [SerializeField] private float emissionIntensity = 2;
    [SerializeField] private UIBar bar;
    private Color fillColor;
    private Color initColor;

    private void Awake()
    {
        initColor = GetComponent<Image>().color;
        fillColor = bar.GetComponent<Image>().color;
        bar.OnValueUpdated += UpdateColor;
        UpdateColor(0);
    }

    private void OnDestroy()
    {
        bar.OnValueUpdated -= UpdateColor;
    }

    private void UpdateColor(float value)
    {
        configurator.SetIntensityPercentage(value);
        if(overrideEmissionColor) 
            configurator.SetTint(Color.Lerp(initColor, fillColor, value), emissionIntensity);
    }
}
