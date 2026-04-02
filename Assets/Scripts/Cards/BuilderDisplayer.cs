using System;
using TMPro;
using UnityEngine;

public class BuilderDisplayer : InfoDisplayer
{
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private GrimoireInfoPanel infoPanel;
    public static event Action<BuilderDisplayer> OnCardChosen;
    public CardDefinition Card => Definition as CardDefinition;

    private void Start()
    {
        infoPanel.gameObject.SetActive(false);
    }

    public override void SetInfo(ADefinition definition)
    {
        base.SetInfo(definition);
        description.text = definition.Description;
    }

    public override void Highlight(bool highlight)
    {
        base.Highlight(highlight);
        EnableInfoPanel(highlight);
    }

    private void EnableInfoPanel(bool enable)
    {
        infoPanel.gameObject.SetActive(enable);
        if (!enable) return;
        infoPanel.SetInfo(Card);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        float newX = Mathf.Abs(infoPanel.transform.localPosition.x) *
            (screenPos.x > Screen.width * 0.5f ? -1 : 1);
        infoPanel.transform.localPosition = new(newX, infoPanel.transform.localPosition.y,
            infoPanel.transform.localPosition.z);
    }

    public void OnButtonClicked() => OnCardChosen?.Invoke(this);
}
