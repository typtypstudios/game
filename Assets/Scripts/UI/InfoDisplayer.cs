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
    private Material emissiveMat;
    private int originalSiblingIndex = -1;
    public ADefinition Definition { get; private set; }

    private void Awake()
    {
        image = GetComponent<Image>();
        emissiveMat = new(image.material);
        image.material = emissiveMat; //Una copia para cada uno, ya que no hay mat prop block para UI
        writableButton = GetComponent<WritableButton>();
        originalNameColor = cardName.color;
    }

    public virtual void SetInfo(ADefinition definition)
    {
        image.sprite = definition.Image;
        writableButton.OverrideText(definition.Name);
        Definition = definition;
    }

    public virtual void Highlight(bool highlight)
    {
        if (highlight && !hovered)
        {
            originalSiblingIndex = transform.GetSiblingIndex();
            transform.localScale *= hoverSizeMult;
            hovered = true;
            cardName.color = writableButton.FillColor + Color.white * highlightColorAddition; //Ligeramente mas claro
            transform.SetAsLastSibling();
            emissiveMat.SetFloat("_EmissionForce", 1);
        }
        else if (!highlight && hovered)
        {
            transform.localScale /= hoverSizeMult;
            hovered = false;
            cardName.color = originalNameColor;

            int targetSiblingIndex = originalSiblingIndex;
            if (transform.parent != null)
            {
                targetSiblingIndex = Mathf.Clamp(targetSiblingIndex, 0, transform.parent.childCount - 1);
                transform.SetSiblingIndex(targetSiblingIndex);
            }

            originalSiblingIndex = -1;
            emissiveMat.SetFloat("_EmissionForce", 0);
        }
    }
}
