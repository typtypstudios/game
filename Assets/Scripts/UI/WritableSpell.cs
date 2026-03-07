using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class WritableSpell : AInputListener
{
    [SerializeField] private bool resetIfFailed = true;
    private TMP_Text spellText;
    private string originalText;
    private int textLength;
    private int idx;

    public UnityEvent<string> OnSpellComplete = new();

    private void Awake()
    {
        spellText = GetComponentInChildren<TMP_Text>();
        textLength = spellText.text.Length;
        originalText = spellText.text;
    }

    public void SetText(string text)
    {
        originalText = text;
        textLength = text.Length;
        ResetText();
    }

    private void ResetText()
    {
        spellText.text = originalText;
        idx = 0;
    }

    protected override void ProcessInput(char c)
    {
        if (originalText[idx] == c)
        {
            spellText.text = fillColorTag + originalText[..(idx + 1)] + "</color>" + 
                originalText[(idx + 1)..];
            if (++idx == textLength)
            {
                OnSpellComplete?.Invoke(originalText);
                ResetText();
            }
        }
        else if(resetIfFailed) ResetText();
    }
}
