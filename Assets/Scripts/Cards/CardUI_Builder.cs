using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CardUI_Builder : MonoBehaviour
{
    private Image image;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private float hoverSizeMult = 1.5f;
    public static event Action<CardUI_Builder> OnCardChosen;
    private bool hovered = false;
    private WritableButton writableButton;
    public CardDefinition Card { get; private set; }

    private void Awake()
    {
        image = GetComponent<Image>();
        writableButton = GetComponent<WritableButton>();
    }

    public void SetCard(CardDefinition card)
    {
        Card = card;
        image.sprite = card.CardImage;
        cardName.text = card.CardName;
        description.text = card.Description;
        writableButton.OverrideText(card.name);
    }

    public void SetHover(bool hover)
    {
        if (hover && !hovered)
        {
            transform.localScale *= hoverSizeMult;
            hovered = true;
            transform.SetAsLastSibling();
        }
        else if(!hover && hovered)
        {
            transform.localScale /= hoverSizeMult;
            hovered = false;
            transform.SetAsFirstSibling();
        }
    }

    public void OnButtonClicked() => OnCardChosen?.Invoke(this);
}
