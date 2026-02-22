using UnityEngine;
using TMPro;
using System;
using TypTyp.TextSystem;

public class RitualManager : AInputListener
{
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private bool dottedSpaces; //Muestra los espacios con � o similar
    private TMP_Text ritualText;
    private string originalText; //Texto original a completar, no tiene por qu� ser igual que el del TMPro
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
        textProvider = GetComponent<ITextProvider>();
    }

    void Start()
    {
        if (textProvider != null) GetNextText();
        else Debug.LogError("No ITextProvider found on RitualManager GameObject.");
    }

    protected override void ProcessInput(char c)
    {
        if (dottedSpaces && c == ' ') c = '-';
        if (c == originalText[charIdx])
        {
            charIdx++;
            isErrorDisplayed = false;
            ritualText.text = fillColorTag + originalText[..charIdx] +
                "</color>" + originalText[charIdx..];
            OnCorrectChar?.Invoke();
        }
        else if (!isErrorDisplayed)
        {
            isErrorDisplayed = true;
            string original = ritualText.text;
            ritualText.text = fillColorTag + originalText[..charIdx] + "</color>" +
                wrongColorTag + originalText[charIdx] + "</color>" + originalText[(charIdx + 1)..];
            OnWrongChar?.Invoke();
        }
        if (charIdx >= originalText.Length)
        {
            numTextsCompleted++;
            GetNextText();
        }
        UpdateProgress();
    }

    private void GetNextText()
    {
        originalText = textProvider.GetNextText();
        if (dottedSpaces) originalText = originalText.Replace(" ", "-");
        ritualText.text = originalText;
        charIdx = 0;
    }

    private void UpdateProgress()
    {
        float globalProgress = (float)numTextsCompleted / TypTyp.Settings.Instance.MaxTextsProvided;
        float localProgress = (float)charIdx / (originalText.Length * TypTyp.Settings.Instance.MaxTextsProvided);
        float progress = globalProgress + localProgress;
        OnProgressUpdated?.Invoke(progress);
    }
}