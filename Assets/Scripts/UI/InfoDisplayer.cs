using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private float hoverSizeMult = 1.5f;
    [SerializeField] private float highlightColorAddition = 0.3f; //Para un color mas claro que fill color para highlight

    [Header("Card Presenter")]
    [SerializeField] private CardVisualPresenter cardVisualPresenter;
    [SerializeField] private Image presenterBorderImage;
    [SerializeField] private float presenterBorderHighlightAddition = 0.2f;

    private Image image;
    private bool hovered = false;
    private bool usingCardPresenter = false;
    private WritableButton writableButton;
    private Color originalNameColor = Color.white;
    private Color originalPresenterBorderColor = Color.white;
    private Material emissiveMat;
    public ADefinition Definition { get; private set; }

    private void Awake()
    {
        image = GetComponent<Image>();
        if (image)
        {
            emissiveMat = new(image.material);
            image.material = emissiveMat; //Una copia para cada uno, ya que no hay mat prop block para UI
        }

        writableButton = GetComponent<WritableButton>();
        originalNameColor = cardName.color;
        if (presenterBorderImage)
        {
            originalPresenterBorderColor = presenterBorderImage.color;
        }
    }

    public virtual void SetInfo(ADefinition definition)
    {
        bool usePresenter = definition is CardDefinition && cardVisualPresenter;
        SetVisualMode(usePresenter);

        if (usePresenter)
        {
            var cardDefinition = definition as CardDefinition;
            int resolvedManaCost = Mathf.Max(0, cardDefinition.ManaCost);
            cardVisualPresenter.SetCard(cardDefinition, resolvedManaCost, resolvedManaCost);
            usingCardPresenter = true;
            writableButton.OverrideText(definition.Name);
            Definition = definition;
            return;
        }

        usingCardPresenter = false;
        cardVisualPresenter?.Clear();

        if (image)
        {
            image.sprite = definition.Image;
        }

        writableButton.OverrideText(definition.Name);
        Definition = definition;

        if (presenterBorderImage)
        {
            presenterBorderImage.color = originalPresenterBorderColor;
        }
    }

    private void SetVisualMode(bool usePresenter)
    {
        if (image)
        {
            // image.gameObject.SetActive(!usePresenter);
            image.enabled = !usePresenter;
        }

        if (cardVisualPresenter)
        {
            cardVisualPresenter.gameObject.SetActive(usePresenter);
        }
    }

    public virtual void Highlight(bool highlight)
    {
        if (highlight && !hovered)
        {
            transform.localScale *= hoverSizeMult;
            hovered = true;
            cardName.color = writableButton.GetButtonColor() + Color.white * highlightColorAddition;

            if (usingCardPresenter && presenterBorderImage)
            {
                originalPresenterBorderColor = presenterBorderImage.color;
                presenterBorderImage.color = originalPresenterBorderColor + Color.white * presenterBorderHighlightAddition;
            }
            else
            {
                if (emissiveMat)
                    emissiveMat.SetFloat("_EmissionForce", 1);
            }

            transform.SetAsLastSibling();
        }
        else if (!highlight && hovered)
        {
            transform.localScale /= hoverSizeMult;
            hovered = false;
            cardName.color = originalNameColor;

            if (usingCardPresenter && presenterBorderImage)
            {
                presenterBorderImage.color = originalPresenterBorderColor;
            }
            else
            {
                if (emissiveMat)
                    emissiveMat.SetFloat("_EmissionForce", 0);
            }

            transform.SetAsFirstSibling();
        }
    }
}
