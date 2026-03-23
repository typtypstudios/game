using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireInfoPanel : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Color durationHighlightColor;
    [SerializeField] private Color positiveHighlightColor = Color.green;
    [SerializeField] private Color negativeHighlightColor = Color.red;
    private string durationColor;
    private string positiveColor;
    private string negativeColor;


    [SerializeField] private StatusEffectDefinition startInfo;//CAMBIAR LUEGO PARA QUE COJA AUTOMÁTICAMENTE PRIMERA CARTA


    private void Start()
    {
        durationColor = Utils.ColorToTag(durationHighlightColor);
        positiveColor = Utils.ColorToTag(positiveHighlightColor);
        negativeColor = Utils.ColorToTag(negativeHighlightColor);
        SetInfo(startInfo);
    }

    public void SetInfo(CardDefinition card)
    {
        image.sprite = card.CardImage;
        nameText.text = card.CardName;
        infoText.text = card.Description;
    }

    public void SetInfo(StatusEffectDefinition effect)
    {
        image.sprite = effect.ImageUI;
        nameText.text = effect.EffectName;
        infoText.text = effect.Description;
        infoText.text = FillStatusInfo(effect);
    }

    private string FillStatusInfo(StatusEffectDefinition effect)
    {
        string desc = effect.Description;
        if (effect.DurationValue == 1) desc = desc.Replace("seconds", "second").Replace("lines", "line");
        desc = desc.Replace("<duration>", durationColor + effect.DurationValue + "</color>");
        string polarityColor = effect.EffectPolarityType == EffectPolarityType.Bad ? 
            negativeColor : positiveColor;
        desc = desc.Replace("<default>", polarityColor + effect.GetDefaultValue() + "</color>");
        return desc;
    }
}
