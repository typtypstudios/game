using UnityEngine;
using TMPro;

[ExecuteAlways]
[RequireComponent(typeof(TextMeshProUGUI))]
public class BubbleMaxWidth : MonoBehaviour
{
    public float minWidth = 35f;
    public float maxWidth = 250f;

    private RectTransform rectTransform;
    private TextMeshProUGUI textComponent;

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (textComponent == null || rectTransform == null) return;

        float naturalWidth = textComponent.preferredWidth;
        float targetWidth = Mathf.Clamp(naturalWidth, minWidth, maxWidth);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
    }
}