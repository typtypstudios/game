using TMPro;
using UnityEngine;

public class BuilderDisplayerBlocker : MonoBehaviour
{
    [SerializeField] private TMP_Text tmp;
    [SerializeField] private WritableButton cardButton;
    [SerializeField] private Color levelHighlightColor;
    private string defaultText = null;
    private string colorTag = null;

    public void CheckBlock(CardDefinition card)
    {
        defaultText ??= tmp.text;
        colorTag ??= Utils.ColorToTag(levelHighlightColor);
        bool block = card.Cult != null && card.RequiredLevel > RuntimeVariables.Instance.CurrentLevel;
        tmp.text = defaultText.Replace("<level>", colorTag + card.RequiredLevel + "</color>");
        cardButton.CompletelyBlock(block);
        this.gameObject.SetActive(block);
    }
}
