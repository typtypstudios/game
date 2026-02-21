using UnityEngine;
using TMPro;
using System;
using TypTyp.TextSystem;

public class RitualManager : AInputListener
{
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private bool dottedSpaces;
    [SerializeField] private int maxTextsProvided = 10;
    private Player player;
    private TMP_Text ritualText;
    private string originalText;
    private ITextProvider textProvider;
    private string wrongColorTag;
    private int idx = 0;
    private int numTextsCompleted = 0;
    bool isErrorDisplayed = false;

    public event Action OnCorrectChar;
    public event Action OnWrongChar;

    void Awake()
    {
        player = GetComponentInParent<Player>();
        wrongColorTag = $"<color #{ColorUtility.ToHtmlStringRGB(wrongColor)}>";
        ritualText = GetComponent<TMP_Text>();
        textProvider = GetComponent<ITextProvider>();
    }

    void Start()
    {
        if(textProvider != null) GetNextText();
        else Debug.LogError("No ITextProvider found on RitualManager GameObject.");
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
        if (idx >= originalText.Length)
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
        idx = 0;
    }

    float prevProgress;
    private void UpdateProgress()
    {
        float globalProgress = (float)numTextsCompleted / maxTextsProvided;
        float localProgress = (float)idx / (originalText.Length * maxTextsProvided);
        float progress = globalProgress + localProgress;
        if (progress == prevProgress) return;
        prevProgress = progress;
        player.UpdateRitualProgress(progress);
    }
}