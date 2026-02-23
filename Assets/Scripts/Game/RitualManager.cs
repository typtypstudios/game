using UnityEngine;
using TMPro;
using System;
using TypTyp.TextSystem;
using TypTyp;

public class RitualManager : AInputListener
{
    [SerializeField] private Color wrongColor = Color.red;
    private TMP_Text ritualText;
    public string OriginalText { get; set; }
    private ITextProvider textProvider;
    private string wrongColorTag;
    private int charIdx = 0;
    private int numTextsCompleted = 0;
    bool isErrorDisplayed = false;

    public event Action OnCorrectChar;
    public event Action OnWrongChar;
    public event Action<float> OnProgressUpdated;

    void Awake()
    {
        wrongColorTag = $"<color #{ColorUtility.ToHtmlStringRGB(wrongColor)}>";
        ritualText = GetComponent<TMP_Text>();
        textProvider = GetComponentInParent<ITextProvider>();
    }

    protected override void ProcessInput(char c)
    {
        if (OriginalText.Equals(string.Empty)) return;
        if (Settings.Instance.DottedText && c == ' ') c = '-';
        if (c == OriginalText[charIdx])
        {
            charIdx++;
            isErrorDisplayed = false;
            ritualText.text = fillColorTag + OriginalText[..charIdx] +
                "</color>" + OriginalText[charIdx..];
            OnCorrectChar?.Invoke();
        }
        else if (!isErrorDisplayed)
        {
            isErrorDisplayed = true;
            string original = ritualText.text;
            ritualText.text = fillColorTag + OriginalText[..charIdx] + "</color>" +
                wrongColorTag + OriginalText[charIdx] + "</color>" + OriginalText[(charIdx + 1)..];
            OnWrongChar?.Invoke();
        }
        if (charIdx >= OriginalText.Length)
        {
            numTextsCompleted++;
            GetNextText();
        }
        UpdateProgress();
    }

    private void GetNextText()
    {
        OriginalText = textProvider.GetNextText();
        ritualText.text = OriginalText;
        charIdx = 0;
    }

    private void UpdateProgress()
    {
        float globalProgress = (float)numTextsCompleted / TypTyp.Settings.Instance.MaxTextsProvided;
        float localProgress = OriginalText.Length == 0 ? 0 : 
            (float)charIdx / (OriginalText.Length * TypTyp.Settings.Instance.MaxTextsProvided);
        float progress = globalProgress + localProgress;
        OnProgressUpdated?.Invoke(progress);
    }
}