using TypTyp;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SkullSprite : MonoBehaviour
{
    [SerializeField] private EmissiveImageConfigurator configurator;
    [SerializeField] private bool overrideEmissionColor = true;
    [SerializeField] private float emissionIntensity = 2;
    private Color fillColor;
    private Color initColor;
    private Player player;

    private void Awake()
    {
        initColor = GetComponent<Image>().color;
    }

    private void OnDestroy()
    {
        if(player) player.CurrentCorruption.OnValueChanged -= UpdateColor;
    }

    public void BindPlayerAndColor(Player player, Color color)
    {
        fillColor = color;            
        this.player = player;
        player.CurrentCorruption.OnValueChanged += UpdateColor;
        UpdateColor(0, 0);
    }

    private void UpdateColor(float prevCorr, float newCorr)
    {
        float value = newCorr / Settings.Instance.MaxCorruption;
        configurator.SetIntensityPercentage(value);
        if(overrideEmissionColor) 
            configurator.SetTint(Color.Lerp(initColor, fillColor, value), emissionIntensity);
    }
}
