using UnityEngine;
using TMPro;
using System;

public class BuilderDisplayer : InfoDisplayer
{
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private GameObject infoButton;
    public static event Action<BuilderDisplayer> OnCardChosen;
    public CardDefinition Card => Definition as CardDefinition;

    private void Start()
    {
        infoButton.SetActive(false);
    }

    public override void SetInfo(ADefinition definition)
    {
        base.SetInfo(definition);
        description.text = definition.Description;
    }

    public override void Highlight(bool highlight)
    {
        base.Highlight(highlight);
        infoButton.SetActive(highlight);
        if (highlight) transform.parent.SetAsLastSibling();
    }

    public void OnButtonClicked() => OnCardChosen?.Invoke(this);
}
