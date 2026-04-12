using System;
using TMPro;
using UnityEngine;
public class BuilderDisplayer : InfoDisplayer
{
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private GrimoireInfoPanel infoPanel;
    [SerializeField] private BuilderDisplayerBlocker blocker;
    public static event Action<BuilderDisplayer> OnCardChosen;
    public CardDefinition Card => Definition as CardDefinition;
    private RectTransform infoPanelRect;
    private Canvas rootCanvas;
    private float infoPanelOffsetYRatio;
    private void Start()
    {
        infoPanelRect = infoPanel != null ? infoPanel.transform as RectTransform : null;
        if (infoPanelRect != null)
        {
            RectTransform parentRect = infoPanelRect.parent as RectTransform;
            if (parentRect != null)
                infoPanelOffsetYRatio = Mathf.Abs(infoPanelRect.localPosition.y) / parentRect.rect.height;
        }
        rootCanvas = GetComponentInParent<Canvas>()?.rootCanvas;
        infoPanel.gameObject.SetActive(false);
    }
    public override void SetInfo(ADefinition definition)
    {
        base.SetInfo(definition);
        description.text = definition.Description;
        blocker.CheckBlock(definition as CardDefinition);
    }
    public override void Highlight(bool highlight)
    {
        base.Highlight(highlight);
        EnableInfoPanel(highlight);
        if (highlight)
            transform.parent.SetAsLastSibling();
    }
    private void EnableInfoPanel(bool enable)
    {
        infoPanel.gameObject.SetActive(enable);
        if (!enable) return;
        infoPanel.SetInfo(Card);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        float direction = screenPos.y > Screen.height * 0.5f ? -1f : 1f;
        RectTransform parentRect = infoPanelRect.parent as RectTransform;
        float offsetY = parentRect != null ? infoPanelOffsetYRatio * parentRect.rect.height : Mathf.Abs(infoPanelRect.localPosition.y);
        infoPanelRect.localPosition = new Vector3(infoPanelRect.localPosition.x, offsetY * direction, infoPanelRect.localPosition.z);
    }
    public void OnButtonClicked() => OnCardChosen?.Invoke(this);
}