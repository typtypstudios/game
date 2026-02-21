using UnityEngine;
using TMPro;
using System;
using TypTyp.TextSystem;
using UnityEngine.Assertions;

public class RitualManager : AInputListener
{
    public float CurrentRitualScore { get; private set; }
    [SerializeField] private Color wrongColor = Color.red;
    private string wrongColorTag;
    private string originalText;
    [SerializeField] private bool dottedSpaces;

    // Events. VFX, SFX and ritual bars may subscribe to these events
    public event Action OnCorrectChar;
    public event Action OnWrongChar;

    private TMP_Text ritualText;
    private ITextProvider textProvider;
    private int idx = 0;
    bool isErrorDisplayed = false;
    // Score variables
    private float ritualScoreUnit = 0.1f;
    private float multiplier = 1.0f;
    private float maxRitualScore = 10f;
    //private UIManager uiManager;

    void Awake()
    {
        wrongColorTag = $"<color #{ColorUtility.ToHtmlStringRGB(wrongColor)}>";
        ritualText = GetComponent<TMP_Text>();
        textProvider = GetComponent<ITextProvider>();
        // Assert.IsNotNull(ritualText, "RitualManager requires a TMP_Text component.");

        OnCorrectChar += AddRitualScore;
        //uiManager = FindFirstObjectByType<UIManager>();
    }

    void Start()
    {
        if(textProvider != null)
        {
            GetNextText();
        }
        else
        {
            Debug.LogError("No ITextProvider found on RitualManager GameObject.");
        }
    }

    protected override void ProcessInput(char c)
    {
        if (dottedSpaces && c == ' ') c = '-'; 
        if (c == originalText[idx])
        {
            idx++;
            isErrorDisplayed = false;
            ritualText.text = fillColorTag + originalText[..idx] + 
                "</color>" + originalText[idx..];
            OnCorrectChar?.Invoke();
        }
        else if(!isErrorDisplayed)
        {
            isErrorDisplayed = true;
            string original = ritualText.text;
            ritualText.text = fillColorTag + originalText[..idx] + "</color>" + 
                wrongColorTag + originalText[idx] + "</color>" + originalText[(idx + 1)..];
            OnWrongChar?.Invoke();
        }
        if (idx >= originalText.Length) GetNextText();
    }

    private void GetNextText()
    {
        originalText = textProvider.GetNextText();
        if (dottedSpaces) originalText = originalText.Replace(" ", "-");
        ritualText.text = originalText;
        idx = 0;
    }

    public void AddRitualScore()
    {
        CurrentRitualScore += ritualScoreUnit * multiplier;
        if (CurrentRitualScore >= maxRitualScore)
        {
            Debug.LogError("Win condition not implemented");
            CurrentRitualScore = maxRitualScore;
        }

        //uiManager.SetRitualProgress(CurrentRitualScore / maxRitualScore);
    }
}