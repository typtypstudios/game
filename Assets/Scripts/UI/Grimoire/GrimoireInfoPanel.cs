using System.Text.RegularExpressions;
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
    [SerializeField] private Color effectHighlightColor = Color.purple;
    private string durationColor;
    private string positiveColor;
    private string negativeColor;
    private string effectColor;

    private void Start()
    {
        durationColor = Utils.ColorToTag(durationHighlightColor);
        positiveColor = Utils.ColorToTag(positiveHighlightColor);
        negativeColor = Utils.ColorToTag(negativeHighlightColor);
        effectColor = Utils.ColorToTag(effectHighlightColor);
    }

    public void SetInfo(CardDefinition card)
    {
        image.sprite = card.CardImage;
        nameText.text = card.CardName;
        infoText.text = FillCardInfo(card);
    }

    private string FillCardInfo(CardDefinition card)
    {
        string desc = card.Description;
        MatchCollection matches = Regex.Matches(desc, @"'([^']*)'");
        desc = desc.Replace("'", "");
        foreach (Match m in matches) 
        {
            string effectName = m.Groups[1].Value;
            desc = desc.Replace(effectName, effectColor + effectName + "</color>");
        }
        matches = Regex.Matches(desc, @"<effectSelf_([a-zA-Z0-9]+)>");
        foreach (Match m in matches)
        {
            int idx = int.Parse(m.Groups[1].Value[0].ToString());
            string effectDesc = FillStatusInfo(card.Spell.OnSelfEffects[idx]);
            if (m.Groups[1].Value.ToLower().Contains("c")) 
                effectDesc = effectDesc.Remove(effectDesc.Length - 1);
            desc = desc.Replace(m.Groups[0].Value, effectDesc);
        }
        matches = Regex.Matches(desc, @"<effectEnemy_([a-zA-Z0-9]+)>");
        foreach (Match m in matches)
        {
            int idx = int.Parse(m.Groups[1].Value[0].ToString());
            string effectDesc = FillStatusInfo(card.Spell.OnEnemyEffects[idx]);
            if (m.Groups[1].Value.ToLower().Contains("c")) 
                effectDesc = effectDesc.Remove(effectDesc.Length - 1);
            desc = desc.Replace(m.Groups[0].Value, effectDesc);
        }
        return desc;
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
        desc = desc.Replace("seconds", "seconds</color>").Replace("lines", "lines</color>");
        if (effect.DurationValue == 1) desc = desc.Replace("seconds", "second").Replace("lines", "line");
        desc = desc.Replace("<duration>", durationColor + effect.DurationValue);
        string polarityColor = effect.EffectPolarityType == EffectPolarityType.Bad ? 
            negativeColor : positiveColor;
        desc = desc.Replace("<value>", polarityColor + effect.GetDefaultValue() + "</color>");
        return desc;
    }
}
