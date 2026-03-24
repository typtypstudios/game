using UnityEngine;
using TMPro;
using System;

public class CardDisplayer_builder : InfoDisplayer
{
    [SerializeField] private TextMeshProUGUI description;
    public static event Action<CardDisplayer_builder> OnCardChosen;
    public CardDefinition Card { get; private set; }

    public override void SetCard(CardDefinition card)
    {
        base.SetCard(card);
        Card = card;
        description.text = card.Description;
    }

    public void OnButtonClicked() => OnCardChosen?.Invoke(this);
}
