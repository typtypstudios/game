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
    private Color presenterBorderBaseColor = Color.white;
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
            presenterBorderBaseColor = presenterBorderImage.color;
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

            if (presenterBorderImage)
            {
                // El color base del borde depende de la carta actual, no del Awake.
                presenterBorderBaseColor = presenterBorderImage.color;
                presenterBorderImage.color = hovered
                    ? GetHighlightedBorderColor(presenterBorderBaseColor)
                    : presenterBorderBaseColor;
            }

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
            presenterBorderImage.color = presenterBorderBaseColor;
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
            if (cardVisualPresenter.gameObject == gameObject)
                cardVisualPresenter.enabled = usePresenter;
            else
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
                presenterBorderBaseColor = presenterBorderImage.color;
                presenterBorderImage.color = GetHighlightedBorderColor(presenterBorderBaseColor);
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
                presenterBorderImage.color = presenterBorderBaseColor;
            }
            else
            {
                if (emissiveMat)
                    emissiveMat.SetFloat("_EmissionForce", 0);
            }

            transform.SetAsFirstSibling();
        }
    }

    private Color GetHighlightedBorderColor(Color baseColor)
    {
        return baseColor + Color.white * presenterBorderHighlightAddition;
    }
}
