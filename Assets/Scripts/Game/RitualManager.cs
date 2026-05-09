using UnityEngine;
using System;
using System.Collections.Generic;
using TypTyp.TextSystem;
using TypTyp.TextSystem.Typable;

public class RitualManager : MonoBehaviour
{
    private TypableController typableController;
    private ITextProvider textProvider;
    private ITextPipeline textPipeline;
    private readonly List<string> processedTexts = new();
    private int numTextsCompleted;
    private int lastIdx;
    private string currentText = "";
    private bool ritualStarted;

    public string CurrentText => currentText;
    public int CurrentLineIndex => numTextsCompleted;
    public int CompletedLines => numTextsCompleted;
    public bool HasStarted => ritualStarted;
    public float Progress { get; private set; }
    private int TargetLineCount
    {
        get
        {
            int configuredMax = TypTyp.Settings.Instance.MaxTextsProvided;
            if (textProvider == null || textProvider.Count <= 0)
                return configuredMax;

            return Mathf.Min(configuredMax, textProvider.Count);
        }
    }

    [Obsolete("Use CurrentText and SetText only for legacy integrations.")]
    public string OriginalText
    {
        get => currentText;
        set => SetText(value);
    }

    public event Action OnCorrectChar;
    public event Action OnWrongChar;
    public event Action<float> OnProgressUpdated;
    public event Action<string> OnTextChanged;
    public event Action<int> OnLineCompleted;
    public event Action OnRitualCompleted;

    [Obsolete("Use OnLineCompleted.")]
    public event Action<int> LineCompleted;

    void Awake()
    {
        typableController = GetComponent<TypableController>();
        textProvider = GetComponentInParent<ITextProvider>();
        textPipeline = GetComponentInParent<ITextPipeline>();
        UnityEngine.Assertions.Assert.IsNotNull(typableController);
    }

    void OnEnable()
    {
        if (typableController == null) return;
        typableController.OnChanged += HandleChanged;
        typableController.OnError += HandleError;
        typableController.OnComplete += HandleComplete;
        MatchManager.OnMatchStarted += StartRitual;
        if (textPipeline != null)
        {
            textPipeline.ProcessorAdded += HandleTextPipelineChanged;
            textPipeline.ProcessorRemoved += HandleTextPipelineChanged;
        }
    }

    void OnDisable()
    {
        if (typableController != null)
        {
            typableController.OnChanged -= HandleChanged;
            typableController.OnError -= HandleError;
            typableController.OnComplete -= HandleComplete;
        }
        if (textPipeline != null)
        {
            textPipeline.ProcessorAdded -= HandleTextPipelineChanged;
            textPipeline.ProcessorRemoved -= HandleTextPipelineChanged;
        }
        MatchManager.OnMatchStarted -= StartRitual;
    }

    public void StartRitual()
    {
        if (ritualStarted) return;

        ritualStarted = true;
        numTextsCompleted = 0;
        InitializeProcessedTextsCache();
        LoadCurrentLine();
        UpdateProgress();
    }

    public void SetText(string text)
    {
        currentText = text ?? "";
        lastIdx = 0;
        if (typableController != null)
            typableController.SetText(currentText);
        OnTextChanged?.Invoke(currentText);
    }

    public string GetText(int index)
    {
        if (index < 0)
            return string.Empty;

        if (textProvider == null)
            return string.Empty;

        if (index >= TargetLineCount || index >= textProvider.Count)
            return string.Empty;

        EnsureProcessedTextsCacheSize();
        string text = processedTexts[index];
        if (text != null)
            return text;

        text = textProvider.GetText(index) ?? string.Empty;
        if (textPipeline != null)
            text = textPipeline.ProcessText(text);

        processedTexts[index] = text;
        return text;
    }

    private void HandleChanged()
    {
        int idx = typableController.Idx;
        if (idx > lastIdx)
        {
            OnCorrectChar?.Invoke();
            UpdateProgress();
        }
        lastIdx = idx;
    }

    private void HandleError()
    {
        OnWrongChar?.Invoke();
    }

    private void HandleComplete()
    {
        numTextsCompleted++;

        if (numTextsCompleted >= TargetLineCount)
        {
            Progress = 1f;
            OnProgressUpdated?.Invoke(Progress);
            OnLineCompleted?.Invoke(numTextsCompleted);
            LineCompleted?.Invoke(numTextsCompleted);
            OnRitualCompleted?.Invoke();
            return;
        }

        LoadCurrentLine();
        OnLineCompleted?.Invoke(numTextsCompleted);
        LineCompleted?.Invoke(numTextsCompleted);
    }

    private void LoadCurrentLine()
    {
        SetText(GetText(numTextsCompleted));
    }

    private void UpdateProgress()
    {
        if (typableController == null) return;
        int targetLineCount = Mathf.Max(1, TargetLineCount);
        float globalProgress = (float)numTextsCompleted / targetLineCount;
        float localProgress = currentText.Length == 0 ? 0 :
            (float)typableController.Idx / (currentText.Length * targetLineCount);
        Progress = Mathf.Clamp01(globalProgress + localProgress);
        OnProgressUpdated?.Invoke(Progress);
    }

    private void HandleTextPipelineChanged(ITextProcessor _)
    {
        InvalidateFutureProcessedTexts();
        // Trigger queue refresh for lines after the current one without touching active line.
        OnTextChanged?.Invoke(currentText);
    }

    private void InvalidateFutureProcessedTexts()
    {
        if (processedTexts.Count == 0)
            return;

        int startIndex = numTextsCompleted + 1;
        int endIndex = Mathf.Min(TargetLineCount, processedTexts.Count);
        for (int key = startIndex; key < endIndex; key++)
        {
            processedTexts[key] = null;
        }
    }

    private void InitializeProcessedTextsCache()
    {
        processedTexts.Clear();
        int cacheSize = TargetLineCount;
        for (int i = 0; i < cacheSize; i++)
        {
            processedTexts.Add(null);
        }
    }

    private void EnsureProcessedTextsCacheSize()
    {
        int targetCount = TargetLineCount;
        while (processedTexts.Count < targetCount)
        {
            processedTexts.Add(null);
        }
    }
}
