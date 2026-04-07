using TMPro;
using UnityEngine;

public class BuilderDisplayerBlocker : MonoBehaviour
{
    [SerializeField] private TMP_Text tmp;
    [SerializeField] private WritableButton cardButton;
    private string defaultText = null;

    public void CheckBlock(CardDefinition card)
    {
        defaultText ??= tmp.text;
        bool block = card.Cult != null && card.RequiredLevel > RuntimeVariables.Instance.CurrentLevel;
        tmp.text = defaultText.Replace("<level>", Utils.ApplyColorToText(card.RequiredLevel.ToString(), 
            RuntimeVariables.Instance.CurrentCult.Color));
        cardButton.CompletelyBlock(block);
        this.gameObject.SetActive(block);
    }
}
