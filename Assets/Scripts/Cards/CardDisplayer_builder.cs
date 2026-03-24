using UnityEngine;
using TMPro;
using System;

public class CardDisplayer_builder : InfoDisplayer
{
    [SerializeField] private TextMeshProUGUI description;
    public static event Action<CardDisplayer_builder> OnCardChosen;
    public CardDefinition Card => Definition as CardDefinition;

    public override void SetInfo(ADefinition definition)
    {
        base.SetInfo(definition);
        description.text = definition.Description;
    }

    public void OnButtonClicked() => OnCardChosen?.Invoke(this);
}
