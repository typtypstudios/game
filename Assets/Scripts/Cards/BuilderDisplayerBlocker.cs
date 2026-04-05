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
        SaveState state = SaveManager.Instance.GetState();
        int currentLevel = state.slot.cultData[state.slot.cultId].level;
        bool block = card.Cult != null && card.RequiredLevel > currentLevel;
        tmp.text = defaultText.Replace("<level>", colorTag + card.RequiredLevel + "</color>");
        cardButton.CompletelyBlock(block);
        this.gameObject.SetActive(block);
    }
}
