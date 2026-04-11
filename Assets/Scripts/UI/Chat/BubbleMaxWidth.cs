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
    private ChatMarkerFormatter marker;
    public bool IsEmpty { get; private set; } = true;

    void Awake()
    {
        parentRectTransform = GetComponent<RectTransform>();
        invisibleText = GetComponent<TextMeshProUGUI>();
        invisibleText.textWrappingMode = TextWrappingModes.Normal;

        if (visibleText != null)
            visibleTextRectTransform = visibleText.rectTransform;
        marker = new ChatMarkerFormatter();
    }

    public void SetText(string fullText)
    {
        string marked = fullText ?? "";

        // Le quita las etiquetas o marcadores especiales para tener el texto "limpio".
        string plain = marker.Strip(marked);
        // Calcula qué parte del texto cabe en el máximo de líneas permitidas.
        string visiblePlain = ComputeVisibleTail(plain);
        // Comprueba si al final quedó algo de texto para mostrar.
        IsEmpty = string.IsNullOrEmpty(visiblePlain);

        float natural = invisibleText.GetPreferredValues(visiblePlain, Mathf.Infinity, 0f).x;
        float targetW = Mathf.Clamp(natural, minWidth, maxWidth);

        parentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetW);
        if (visibleTextRectTransform != null)
            visibleTextRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetW);

        invisibleText.text = visiblePlain;
        if (visibleText != null)
        {
            // Vuelve a colocar las etiquetas especiales en el lugar correcto del texto recortado.
            string visibleMarked = AlignMarkersToTruncation(marked, plain, visiblePlain);
            // Convierte esas etiquetas en formato (como colores o negrita) y lo muestra en pantalla.
            visibleText.text = marker.ToRich(visibleMarked);
        }
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

    // Ajusta para que no se vean los marcadores de color si casualmente se llegan a ver
    private string AlignMarkersToTruncation(string marked, string plain, string visiblePlain)
    {
        if (marked == plain) return visiblePlain;
        if (visiblePlain.Length == plain.Length) return marked;

        int removed = plain.Length - visiblePlain.Length;
        int i = 0, skipped = 0;
        while (i < marked.Length && skipped < removed)
        {
            if (marked[i] != marker.SpellMarker) skipped++;
            i++;
        }
        return marked.Substring(i);
    }

    // Se encarga de limitar el número máximo de líneas
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