using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class InfoDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private float hoverSizeMult = 1.5f;
    [SerializeField] private float highlightColorAddition = 0.3f; //Para un color mas claro que fill color para highlight

    [Header("Card Presenter")]
    [SerializeField] private CardVisualPresenter cardVisualPresenter;
    [SerializeField] private Image presenterBorderImage;
    [SerializeField] private float presenterBorderEmissionForce = 1f;

    [Header("Selection Animation")]
    [SerializeField] private float interpolationTime = 0.1f;
    private Vector3 initScale;

    private Image image;
    private bool hovered = false;
    private bool usingCardPresenter = false;
    private WritableButton writableButton;
    private Color originalNameColor = Color.white;
    private Material emissiveMat;
    private Material presenterBorderEmissiveMat;
    public ADefinition Definition { get; private set; }

    private void Awake()
    {
        image = GetComponent<Image>();
        if (image)
        {
            emissiveMat = new(image.material);
            image.material = emissiveMat; //Una copia para cada uno, ya que no hay mat prop block para UI
        }
        initScale = transform.localScale;
        writableButton = GetComponent<WritableButton>();
        originalNameColor = cardName.color;
        if (presenterBorderImage)
        {
            presenterBorderEmissiveMat = new(presenterBorderImage.material);
            presenterBorderImage.material = presenterBorderEmissiveMat;
            SetPresenterBorderEmission(false);
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

            SetPresenterBorderEmission(hovered);

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

        SetPresenterBorderEmission(false);
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
            StopAllCoroutines();
            StartCoroutine(InterpolateScale(initScale * hoverSizeMult));
            hovered = true;
            cardName.color = writableButton.GetButtonColor() + Color.white * highlightColorAddition;

            if (usingCardPresenter && presenterBorderImage)
            {
                SetPresenterBorderEmission(true);
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
            StopAllCoroutines();
            StartCoroutine(InterpolateScale(initScale));
            hovered = false;
            cardName.color = originalNameColor;

            if (usingCardPresenter && presenterBorderImage)
            {
                SetPresenterBorderEmission(false);
            }
            else
            {
                if (emissiveMat)
                    emissiveMat.SetFloat("_EmissionForce", 0);
            }

            transform.SetAsFirstSibling();
        }
    }

    private void SetPresenterBorderEmission(bool enabled)
    {
        if (!presenterBorderEmissiveMat)
        {
            return;
        }

        if (presenterBorderEmissiveMat.HasProperty("_EmissionForce"))
        {
            presenterBorderEmissiveMat.SetFloat("_EmissionForce", enabled ? presenterBorderEmissionForce : 0f);
        }
    }

    IEnumerator InterpolateScale(Vector3 targetScale)
    {
        float dist = Vector3.Distance(transform.localScale, targetScale);
        float speed = dist / interpolationTime;
        while(transform.localScale != targetScale)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, speed * Time.deltaTime);
            yield return null;
        }
    }
}
