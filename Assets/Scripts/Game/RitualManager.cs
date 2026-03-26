using UnityEngine;
using TMPro;
using System;
using TypTyp.TextSystem;
using TypTyp;

public class RitualManager : AInputListener
{
    [SerializeField] private Color wrongColor = Color.red;
    private TMP_Text ritualText;
    public string OriginalText { get; set; } = "";
    private ITextProvider textProvider;
    private string wrongColorTag;
    private int numTextsCompleted = 0;
    bool isErrorDisplayed = false;

    public event Action OnCorrectChar;
    public event Action OnWrongChar;
    public event Action<float> OnProgressUpdated;
    public event Action<int> LineCompleted;

    private bool ritualActive = true;

    void Awake()
    {
        wrongColorTag = Utils.ColorToTag(wrongColor);
        ritualText = GetComponent<TMP_Text>();
        textProvider = GetComponentInParent<ITextProvider>();
    }

    protected override void ProcessInput(char c)
    {
        if (!ritualActive) return;

        if (OriginalText.Equals(string.Empty)) return;
        if (Settings.Instance.ShowSpaces && c == ' ') c = '-';
        if (c == OriginalText[Idx])
        {
            Idx++;
            isErrorDisplayed = false;
            ritualText.text = fillColorTag + OriginalText[..Idx] +
                "</color>" + OriginalText[Idx..];
            OnCorrectChar?.Invoke();
        }
        else if (!isErrorDisplayed)
        {
            isErrorDisplayed = true;
            string original = ritualText.text;
            ritualText.text = fillColorTag + OriginalText[..Idx] + "</color>" +
                wrongColorTag + OriginalText[Idx] + "</color>" + OriginalText[(Idx + 1)..];
            OnWrongChar?.Invoke();
        }
        if (Idx >= OriginalText.Length)
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
        Idx = 0;
        //Esto solo ocurre en cliente
        LineCompleted?.Invoke(numTextsCompleted);
    }

    private void UpdateProgress()
    {
        float globalProgress = (float)numTextsCompleted / TypTyp.Settings.Instance.MaxTextsProvided;
        float localProgress = OriginalText.Length == 0 ? 0 : 
            (float)Idx / (OriginalText.Length * TypTyp.Settings.Instance.MaxTextsProvided);
        float progress = globalProgress + localProgress;
        OnProgressUpdated?.Invoke(progress);
    }

    public void SetActive(bool value)
    {
        ritualActive = value;
    }
}