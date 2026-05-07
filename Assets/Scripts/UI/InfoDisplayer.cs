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
    [SerializeField] private UICardView cardView;

    [Header("Selection Animation")]
    [SerializeField] private float interpolationTime = 0.1f;
    [SerializeField] private bool lastSiblingIfSelected = true;
    private Vector3 initScale;

    private Image image;
    private bool highlighted = false;
    private WritableButton writableButton;
    private Color originalNameColor = Color.white;
    public ADefinition Definition { get; private set; }

    private void Awake()
    {
        image = GetComponent<Image>();
        initScale = transform.localScale;
        writableButton = GetComponent<WritableButton>();
        originalNameColor = cardName.color;
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

            writableButton.OverrideText(definition.Name);
            Definition = definition;
            return;
        }

        cardVisualPresenter?.Clear();

        if (image)
        {
            image.sprite = definition.Image;
        }

        writableButton.OverrideText(definition.Name);
        Definition = definition;
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
        if (cardView != null) cardView.Details.color = highlight ? cardView.Border.color : Color.white;
        if (highlight && !highlighted)
        {
            StopAllCoroutines();
            StartCoroutine(InterpolateScale(initScale * hoverSizeMult));
            highlighted = true;
            cardName.color = writableButton.GetButtonColor() + Color.white * highlightColorAddition;

            foreach(var config in GetComponentsInChildren<EmissiveImageConfigurator>(true))
                config.ToggleEmission(true);

            if (lastSiblingIfSelected) transform.SetAsLastSibling();
        }
        else if (!highlight && highlighted)
        {
            StopAllCoroutines();
            StartCoroutine(InterpolateScale(initScale));
            highlighted = false;
            cardName.color = originalNameColor;

            foreach (var config in GetComponentsInChildren<EmissiveImageConfigurator>(true))
                config.ToggleEmission(false);

            //transform.SetAsFirstSibling();
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
