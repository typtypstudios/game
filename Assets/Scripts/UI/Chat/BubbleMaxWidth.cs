using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class BubbleMaxWidth : MonoBehaviour
{
    public float minWidth = 35f;
    public float maxWidth = 250f;
    public int maxLines = 2;

    private RectTransform parentRectTransform;
    private RectTransform visibleTextRectTransform;
    private TextMeshProUGUI invisibleText;
    [SerializeField] private TextMeshProUGUI visibleText;

    public bool IsEmpty { get; private set; } = true;

    void Awake()
    {
        parentRectTransform = GetComponent<RectTransform>();
        invisibleText = GetComponent<TextMeshProUGUI>();
        invisibleText.textWrappingMode = TextWrappingModes.Normal;

        if (visibleText != null)
            visibleTextRectTransform = visibleText.rectTransform;
    }

    public void SetText(string fullText)
    {
        string invisible = ComputeVisibleTail(fullText);
        invisibleText.text = invisible;
        IsEmpty = string.IsNullOrEmpty(invisible);

        float natural = invisibleText.GetPreferredValues(invisible, Mathf.Infinity, 0f).x;
        float targetW = Mathf.Clamp(natural, minWidth, maxWidth);
        parentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetW);

        // Ajustar el tamaño del visible rect transform a exacatmente el del padre
        // El vertical es adaptativo y debe ser igual al del padre, que es el que se adapta automáticamente
        visibleTextRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetW);
        visibleText.text = invisible;
    }

    public void Clear()
    {
        if (invisibleText == null) return;
        invisibleText.text = "";
        IsEmpty = true;
        parentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minWidth);

        if (visibleText != null)
        {
            visibleText.text = "";
            visibleTextRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minWidth);
        }
    }

    private string ComputeVisibleTail(string fullText)
    {
        if (string.IsNullOrEmpty(fullText)) return "";

        parentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);

        invisibleText.text = fullText;
        invisibleText.ForceMeshUpdate();

        var info = invisibleText.textInfo;
        int lineCount = info.lineCount;

        if (lineCount <= maxLines) return fullText;

        int firstVisibleLine = lineCount - maxLines;
        int startCharIndex = info.lineInfo[firstVisibleLine].firstCharacterIndex;

        return fullText.Substring(startCharIndex);
    }
}