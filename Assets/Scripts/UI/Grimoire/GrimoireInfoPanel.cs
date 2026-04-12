using System.Text.RegularExpressions;
using TMPro;
using TypTyp;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireInfoPanel : MonoBehaviour, INavigationCtxReceiver, INavigationLeaveReceiver
{
    [Header("Legacy")]
    [SerializeField] private Image image;
    [SerializeField] private float emission = 0.9f;

    [Header("Card Presenter")]
    [SerializeField] private CardVisualPresenter cardVisualPresenter;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text infoText;
    private string durationTag;
    private string positiveTag;
    private string negativeTag;
    private string effectTag;
    private bool isActive = false;
    public string DisplayedName => nameText.text;

    private void Awake()
    {
        if (image != null)
        {
            image.material = new(image.material);
            image.material.SetFloat("_EmissionForce", emission);
        }
        durationTag = Utils.ColorToTag(UIColors.Instance.DurationHighlightColor);
        positiveTag = Utils.ColorToTag(UIColors.Instance.PositiveHighlightColor);
        negativeTag = Utils.ColorToTag(UIColors.Instance.NegativeHighlightColor);
        effectTag = Utils.ColorToTag(UIColors.Instance.EffectHighlightColor);
    }

    public void ReceiveContext(Screens previousScreen, bool isGoingBack)
    {
        isActive = true;
    }

    public void OnLeave()
    {
        isActive = false;
        //cardVisualPresenter?.Clear();
    }

    public void SetInfo(ADefinition definition)
    {
        bool usePresenter = definition is CardDefinition && cardVisualPresenter;
        SetVisualMode(usePresenter);

        if (usePresenter)
        {
            var cardDefinition = definition as CardDefinition;
            int resolvedManaCost = Mathf.Max(0, cardDefinition.ManaCost);
            cardVisualPresenter.SetCard(cardDefinition, resolvedManaCost, resolvedManaCost);
        }
        else
        {
            cardVisualPresenter?.Clear();
            if (image != null) image.sprite = definition.Image;
        }

        if (nameText != null) nameText.text = definition.Name;
        infoText.text = definition is CardDefinition ? FillCardInfo(definition as CardDefinition) :
            FillStatusInfo(definition as StatusEffectDefinition);
        if (isActive && AudioManager.Instance != null)
            AudioManager.Instance.PlayUI(UISound.SelectCard);
    }

    private void SetVisualMode(bool usePresenter)
    {
        if (image != null)
        {
            image.enabled = !usePresenter;
        }

        if (cardVisualPresenter)
        {
            if (cardVisualPresenter.gameObject == gameObject)
                cardVisualPresenter.enabled = usePresenter;
            else
                cardVisualPresenter.gameObject.SetActive(usePresenter);
        }
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
