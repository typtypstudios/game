using TypTyp;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SkullSprite : MonoBehaviour
{
    [SerializeField] private EmissiveImageConfigurator configurator;
    private Image image;
    private Color fillColor;
    private Color initColor;
    private Player player;

    private void Awake()
    {
        image = GetComponent<Image>();
        initColor = image.color;
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
        image.color = Color.Lerp(initColor, fillColor, value);
    }
}
