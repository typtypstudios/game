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
    [SerializeField] private float highlightColorAddition = 0.3f; //Para un color más claro que fill color para highlight
    public static event Action<CardUI_Builder> OnCardChosen;
    private bool hovered = false;
    private WritableButton writableButton;
    private Color originalNameColor = Color.white;
    public CardDefinition Card { get; private set; }

    private void Awake()
    {
        image = GetComponent<Image>();
        writableButton = GetComponent<WritableButton>();
        originalNameColor = cardName.color;
    }

    public void SetCard(CardDefinition card)
    {
        Card = card;
        image.sprite = card.CardImage;
        description.text = card.Description;
        writableButton.OverrideText(card.CardName);
    }

    public void Highlight(bool highlight)
    {
        if (highlight && !hovered)
        {
            transform.localScale *= hoverSizeMult;
            hovered = true;
            cardName.color = writableButton.FillColor + Color.white * highlightColorAddition; //Ligeramente más claro
            transform.SetAsLastSibling();
        }
        else if(!highlight && hovered)
        {
            transform.localScale /= hoverSizeMult;
            hovered = false;
            cardName.color = originalNameColor;
            transform.SetAsFirstSibling();
        }
    }

    public void OnButtonClicked() => OnCardChosen?.Invoke(this);
}
