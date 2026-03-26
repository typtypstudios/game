using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class WritableText : AInputListener
{
    [Range(0, 1)][SerializeField] private float minColor;
    [SerializeField] private bool startRandomized = true;
    [SerializeField] private bool fixedSeedBasedOnText = false;
    [SerializeField] private bool resetIfFail = false;
    private TextMeshProUGUI text;
    private string originalText;
    private Canvas canvas;
    private int seed;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        originalText = text.text.Trim();
        seed = originalText.ToUpper().GetHashCode();
        if(startRandomized) OnStringTyped(true);
        canvas = GetComponentInParent<Canvas>();
    }

    public void ResetText() => originalText = text.text.Trim();
        
    private void OnStringTyped(bool onlyRandomize = false)
    {
        if(!onlyRandomize) text.color = FillColor;
        FillColor = Utils.GetDifferentColor(FillColor, fixedSeedBasedOnText ? seed : -1);
        Idx = 0;
    }

    protected override void ProcessInput(char c)
    {
        if (!canvas.enabled) return;
        if (originalText[Idx].ToString().ToLower().Equals(c.ToString().ToLower()))
        {
            text.text = fillColorTag + originalText[..(Idx + 1)] + "</color>" + originalText[(Idx + 1)..];
            if (++Idx == originalText.Length) OnStringTyped();
        }
        else if(resetIfFail)
        {
            text.text = originalText;
            Idx = 0;
        }
    }
}
