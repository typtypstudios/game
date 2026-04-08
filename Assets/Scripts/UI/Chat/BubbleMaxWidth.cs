using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class BubbleMaxWidth : MonoBehaviour
{
    public float minWidth = 35f;
    public float maxWidth = 250f;
    public int maxLines = 2;

    private RectTransform rectTransform;
    private TextMeshProUGUI textComponent;

    public bool IsEmpty { get; private set; } = true;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        textComponent = GetComponent<TextMeshProUGUI>();
        textComponent.textWrappingMode = TextWrappingModes.Normal;
    }

    public void SetText(string fullText)
    {
        if (textComponent == null) return;

        string visible = ComputeVisibleTail(fullText);
        textComponent.text = visible;
        IsEmpty = string.IsNullOrEmpty(visible);

        float natural = textComponent.GetPreferredValues(visible, Mathf.Infinity, 0f).x;
        float target = Mathf.Clamp(natural, minWidth, maxWidth);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, target);
    }

    public void Clear()
    {
        if (textComponent == null) return;
        textComponent.text = "";
        IsEmpty = true;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minWidth);
    }

    private string ComputeVisibleTail(string fullText)
    {
        if (string.IsNullOrEmpty(fullText)) return "";

        // Fijar el ancho al máximo para que TMP haga el wrap real
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);

        // Asignar el texto completo y forzar el layout
        textComponent.text = fullText;
        textComponent.ForceMeshUpdate();

        var info = textComponent.textInfo;
        int lineCount = info.lineCount;

        // Si ya cabe, devolver tal cual
        if (lineCount <= maxLines) return fullText;

        // Queremos quedarnos con las últimas `maxLines` líneas.
        // La línea a partir de la cual empieza lo visible:
        int firstVisibleLine = lineCount - maxLines;
        int startCharIndex = info.lineInfo[firstVisibleLine].firstCharacterIndex;

        return fullText.Substring(startCharIndex);
    }
}