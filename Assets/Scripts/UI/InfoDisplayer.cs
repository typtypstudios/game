using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private float hoverSizeMult = 1.5f;
    [SerializeField] private float highlightColorAddition = 0.3f; //Para un color mas claro que fill color para highlight
    private Image image;
    private bool hovered = false;
    private WritableButton writableButton;
    private Color originalNameColor = Color.white;

    private void Awake()
    {
        image = GetComponent<Image>();
        writableButton = GetComponent<WritableButton>();
        originalNameColor = cardName.color;
    }

    public virtual void SetCard(CardDefinition card)
    {
        image.sprite = card.CardImage;
        writableButton.OverrideText(card.CardName);
    }

    public virtual void SetEffect(StatusEffectDefinition effect)
    {
        image.sprite = effect.ImageUI;
        writableButton.OverrideText(effect.EffectName);
    }

    public void Highlight(bool highlight)
    {
        if (highlight && !hovered)
        {
            transform.localScale *= hoverSizeMult;
            hovered = true;
            cardName.color = writableButton.FillColor + Color.white * highlightColorAddition; //Ligeramente m�s claro
            transform.SetAsLastSibling();
        }
        else if (!highlight && hovered)
        {
            transform.localScale /= hoverSizeMult;
            hovered = false;
            cardName.color = originalNameColor;
            transform.SetAsFirstSibling();
        }
    }
}
