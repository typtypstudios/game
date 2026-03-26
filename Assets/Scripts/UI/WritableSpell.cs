using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class WritableSpell : AInputListener
{
    [SerializeField] private bool resetIfFailed = true;
    private TMP_Text spellText;
    private string originalText;
    private int textLength;

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
        Idx = 0;
    }

    protected override void ProcessInput(char c)
    {
        if (originalText[Idx] == c)
        {
            spellText.text = fillColorTag + originalText[..(Idx + 1)] + "</color>" + 
                originalText[(Idx + 1)..];
            if (++Idx == textLength)
            {
                OnSpellComplete?.Invoke(originalText);
                ResetText();
            }
        }
        else if(resetIfFailed) ResetText();
    }
}
