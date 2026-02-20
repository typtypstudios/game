using UnityEngine;
using TMPro;
using System;
using TypTyp.TextSystem;
using UnityEngine.Assertions;

public class RitualManager : AInputListener
{
    public float CurrentRitualScore { get; private set; }

    [SerializeField] private TMP_Text filledText;
    [SerializeField] private bool dottedSpaces;

    // Events. VFX, SFX and ritual bars may subscribe to these events
    public event Action OnCorrectChar;
    public event Action OnErrorChar;


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
        ritualText = GetComponent<TMP_Text>();
        textProvider = GetComponent<ITextProvider>();
        // Assert.IsNotNull(ritualText, "RitualManager requires a TMP_Text component.");
        if(dottedSpaces) ritualText.text = ritualText.text.Replace(" ", "-");

        OnCorrectChar += AddRitualScore;
        //uiManager = FindFirstObjectByType<UIManager>();
    }

    void Start()
    {
        if(textProvider != null)
        {
            ritualText.text = textProvider.GetNextText();
            if (dottedSpaces) ritualText.text = ritualText.text.Replace(" ", "-");
        }
        else
        {
            Debug.LogError("No ITextProvider found on RitualManager GameObject.");
        }
    }

    readonly string colorTag = "<color #FF0000>";
    protected override void ProcessInput(char c)
    {
        if (idx == ritualText.text.Length) return;
        if (dottedSpaces && c == ' ') c = '-'; 
        if (c == ritualText.text[idx])
        {
            filledText.text += c;
            idx++;
            if (isErrorDisplayed)
            {
                ritualText.text = ritualText.text.Replace("</color>", "").Replace(colorTag, "");
                idx -= colorTag.Length;
                isErrorDisplayed = false;
            }

            // Invoke the event
            OnCorrectChar?.Invoke();
        }
        else if(!isErrorDisplayed)
        {
            string original = ritualText.text;
            ritualText.text = original[..idx] + colorTag + 
                original[idx] + "</color>" + original[(idx + 1)..];
            idx += colorTag.Length;
            isErrorDisplayed = true;

            // Invoke the event
            OnErrorChar?.Invoke();
        }
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