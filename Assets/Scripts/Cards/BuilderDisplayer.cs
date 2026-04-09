using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuilderDisplayer : InfoDisplayer, ISelectHandler
{
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private GrimoireInfoPanel infoPanel;
    [SerializeField] private BuilderDisplayerBlocker blocker;
    public static event Action<BuilderDisplayer> OnCardChosen;
    public CardDefinition Card => Definition as CardDefinition;

    private RectTransform infoPanelRect;
    private Transform infoPanelOriginalParent;
    private int infoPanelOriginalSiblingIndex;
    private Vector3 infoPanelOriginalLocalPosition;
    private Canvas rootCanvas;
    private float infoPanelOffsetX;
    private float infoPanelOffsetY;

    private void Start()
    {
        infoPanelRect = infoPanel != null ? infoPanel.transform as RectTransform : null;
        if (infoPanelRect != null)
        {
            infoPanelOriginalParent = infoPanelRect.parent;
            infoPanelOriginalSiblingIndex = infoPanelRect.GetSiblingIndex();
            infoPanelOriginalLocalPosition = infoPanelRect.localPosition;
            infoPanelOffsetX = Mathf.Abs(infoPanelOriginalLocalPosition.x);
            infoPanelOffsetY = infoPanelOriginalLocalPosition.y;
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
    }

    private void EnableInfoPanel(bool enable)
    {
        infoPanel.gameObject.SetActive(enable);
        if (!enable)
        {
            RestoreInfoPanelParent();
            return;
        }

        infoPanel.SetInfo(Card);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        float direction = screenPos.x > Screen.width * 0.5f ? -1f : 1f;

        if (infoPanelRect != null && rootCanvas != null)
        {
            RectTransform rootRect = rootCanvas.transform as RectTransform;
            Camera uiCamera = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;

            infoPanelRect.SetParent(rootRect, false);
            infoPanelRect.SetAsLastSibling();

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRect, screenPos, uiCamera, out Vector2 localPoint))
            {
                infoPanelRect.anchoredPosition = new Vector2(localPoint.x + infoPanelOffsetX * direction, localPoint.y + infoPanelOffsetY);
            }
        }
        else
        {
            float newX = Mathf.Abs(infoPanel.transform.localPosition.x) * direction;
            infoPanel.transform.localPosition = new(newX, infoPanel.transform.localPosition.y, infoPanel.transform.localPosition.z);
        }
    }

    private void RestoreInfoPanelParent()
    {
        if (infoPanelRect == null || infoPanelOriginalParent == null)
        {
            return;
        }

        if (infoPanelRect.parent != infoPanelOriginalParent)
        {
            infoPanelRect.SetParent(infoPanelOriginalParent, false);
            int siblingIndex = Mathf.Clamp(infoPanelOriginalSiblingIndex, 0, infoPanelOriginalParent.childCount - 1);
            infoPanelRect.SetSiblingIndex(siblingIndex);
            infoPanelRect.localPosition = infoPanelOriginalLocalPosition;
        }
    }

    public void OnButtonClicked()
    {
        // Seleccionar esta carta
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject);

        OnCardChosen?.Invoke(this);
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Por implementar para que se pueda navegar entre cartas con las flechas del teclado
        //OnCardChosen?.Invoke(this);
    }
}
