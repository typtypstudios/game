using System.Text.RegularExpressions;
using TMPro;
using TypTyp;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireInfoPanel : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float emission = 0.9f;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text infoText;
    private string durationTag;
    private string positiveTag;
    private string negativeTag;
    private string effectTag;
    public string DisplayedName => nameText.text;

    private void Awake()
    {
        if(image != null)
        {
            image.material = new(image.material);
            image.material.SetFloat("_EmissionForce", emission);
        }
        durationTag = Utils.ColorToTag(Settings.Instance.DurationHighlightColor);
        positiveTag = Utils.ColorToTag(Settings.Instance.PositiveHighlightColor);
        negativeTag = Utils.ColorToTag(Settings.Instance.NegativeHighlightColor);
        effectTag = Utils.ColorToTag(Settings.Instance.EffectHighlightColor);
    }

    public void SetInfo(ADefinition definition)
    {
        if(image != null) image.sprite = definition.Image;
        if(nameText != null) nameText.text = definition.Name;
        infoText.text = definition is CardDefinition ? FillCardInfo(definition as CardDefinition) : 
            FillStatusInfo(definition as StatusEffectDefinition);
    }

    private string FillCardInfo(CardDefinition card)
    {
        string desc = card.Description;
        MatchCollection matches = Regex.Matches(desc, @"<effect([a-zA-Z0-9]+)_(\d+)>");
        foreach (Match m in matches) 
        {
            if (m.Groups[1].Value.Contains("Desc")) continue;
            int idx = int.Parse(m.Groups[2].Value);
            string effectName = m.Groups[1].Value.Equals("Self") ? 
                card.Spell.OnSelfEffects[idx].Name : card.Spell.OnEnemyEffects[idx].Name;
            desc = desc.Replace(m.Groups[0].Value, effectTag + effectName + "</color>");
        }
        matches = Regex.Matches(desc, @"<effect([a-zA-Z0-9]+)Desc_([a-zA-Z0-9]+)>");
        foreach (Match m in matches)
        {
            int idx = int.Parse(m.Groups[2].Value[0].ToString());
            string effectDesc = m.Groups[1].Value.Equals("Self") ?
                FillStatusInfo(card.Spell.OnSelfEffects[idx]) : FillStatusInfo(card.Spell.OnEnemyEffects[idx]);
            if (m.Groups[2].Value.ToLower().Contains("c"))
                effectDesc = effectDesc.Remove(effectDesc.Length - 1);
            desc = desc.Replace(m.Groups[0].Value, effectDesc);
        }
        return desc;
    }

    private string FillStatusInfo(StatusEffectDefinition effect)
    {
        string desc = effect.Description;
        desc = desc.Replace("<duration>", durationTag + effect.DurationValue + 
            (effect.DurationType == EffectDurationType.Lines ? " lines" : " seconds") + "</color>");
        if (effect.DurationValue == 1) desc = desc.Replace("seconds", "second").Replace("lines", "line");
        string polarityColor = effect.EffectPolarityType == EffectPolarityType.Bad ? 
            negativeTag : positiveTag;
        desc = desc.Replace("<value>", polarityColor + effect.GetDefaultValue() + "</color>");
        return desc;
    }
}
