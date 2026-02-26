using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class WritableText : AInputListener
{
    [Range(0, 1)][SerializeField] private float minColor;
    private TextMeshProUGUI text;
    private string originalText;
    private int idx; 

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        originalText = text.text;
        OnStringTyped(true);
    }

    private void OnStringTyped(bool onlyRandomize = false)
    {
        if(!onlyRandomize) text.color = FillColor;
        float r = Random.Range(minColor, 1);
        float g = Random.Range(minColor, 1);
        float b = Random.Range(minColor, 1);
        FillColor = new Color(r, g, b);
        idx = 0;
    }

    protected override void ProcessInput(char c)
    {
        if(c == originalText[idx])
        {
            text.text = fillColorTag + originalText[..(idx + 1)] + "</color>" + originalText[(idx + 1)..];
            if (++idx == originalText.Length) OnStringTyped();
        }
    }
}
