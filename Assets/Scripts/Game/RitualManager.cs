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
    private StatusEffectController statusEffectController;
    private readonly List<string> sourceTexts = new();
    private readonly List<ProcessedTextCache> processedTexts = new();
    private int numTextsCompleted;
    private int lastIdx;
    private string currentText = "";
    private bool ritualStarted;
    private bool ritualTextsPrepared;

    public string CurrentText => currentText;
    public int CurrentLineIndex => numTextsCompleted;
    public int CompletedLines => numTextsCompleted;
    public bool HasStarted => ritualStarted;
    public bool HasPreparedTexts => ritualTextsPrepared;
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
        statusEffectController = GetComponentInParent<StatusEffectController>();
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
        if (statusEffectController != null)
        {
            statusEffectController.OnEffectRefreshed.AddListener(HandleStatusEffectRefreshed);
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
        if (statusEffectController != null)
        {
            statusEffectController.OnEffectRefreshed.RemoveListener(HandleStatusEffectRefreshed);
        }
        MatchManager.OnMatchStarted -= StartRitual;
    }

    public void StartRitual()
    {
        if (ritualStarted) return;

        PrepareRitualTexts();
        ritualStarted = true;
        UpdateProgress();
    }

    public void PrepareRitualTexts()
    {
        if (ritualTextsPrepared) return;

        ritualTextsPrepared = true;
        numTextsCompleted = 0;
        InitializeProcessedTextsCache();
        LoadCurrentLine();
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
        string text = GetSourceText(index);
        string processorKey = GetProcessorCacheKey(index);
        ProcessedTextCache cached = processedTexts[index];
        if (cached.IsValid && cached.ProcessorKey == processorKey)
            return cached.Text;

        if (textPipeline != null)
            text = textPipeline.ProcessText(text, processor => ShouldApplyProcessorToLine(processor, index));

        processedTexts[index] = new ProcessedTextCache(text, processorKey);
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

    private void HandleStatusEffectRefreshed(StatusEffect _)
    {
        InvalidateFutureProcessedTexts();
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
            processedTexts[key] = default;
        }
    }

    private void InitializeProcessedTextsCache()
    {
        sourceTexts.Clear();
        processedTexts.Clear();
        int cacheSize = TargetLineCount;
        for (int i = 0; i < cacheSize; i++)
        {
            sourceTexts.Add(null);
            processedTexts.Add(default);
        }
    }

    private void EnsureProcessedTextsCacheSize()
    {
        int targetCount = TargetLineCount;
        while (sourceTexts.Count < targetCount)
        {
            sourceTexts.Add(null);
        }
        while (processedTexts.Count < targetCount)
        {
            processedTexts.Add(default);
        }
    }

    private string GetSourceText(int index)
    {
        string sourceText = sourceTexts[index];
        if (sourceText != null)
            return sourceText;

        sourceText = textProvider.GetText(index) ?? string.Empty;
        sourceTexts[index] = sourceText;
        return sourceText;
    }

    private string GetProcessorCacheKey(int lineIndex)
    {
        if (textPipeline == null)
            return string.Empty;

        IReadOnlyList<ITextProcessor> processors = textPipeline.Processors;
        if (processors == null || processors.Count == 0)
            return string.Empty;

        List<int> appliedProcessors = new();
        for (int i = 0; i < processors.Count; i++)
        {
            ITextProcessor processor = processors[i];
            if (!ShouldApplyProcessorToLine(processor, lineIndex))
                continue;

            appliedProcessors.Add(processor.GetHashCode());
        }

        return string.Join("|", appliedProcessors);
    }

    private bool ShouldApplyProcessorToLine(ITextProcessor processor, int lineIndex)
    {
        if (textPipeline == null || !textPipeline.IsRuntimeProcessor(processor))
            return true;

        List<StatusEffect> effects = statusEffectController != null ? statusEffectController.Effects : null;
        if (effects == null || effects.Count == 0)
            return true;

        bool hasLineProcessorEffect = false;
        for (int i = 0; i < effects.Count; i++)
        {
            StatusEffect effect = effects[i];
            if (effect.Definition is not TextProcessorEffect textProcessorEffect ||
                !ReferenceEquals(textProcessorEffect.Processor, processor))
                continue;

            if (effect.Definition.DurationType != EffectDurationType.Lines)
                return true;

            hasLineProcessorEffect = true;
            if (effect.AffectsLine(lineIndex))
                return true;
        }

        return !hasLineProcessorEffect;
    }

    private readonly struct ProcessedTextCache
    {
        public readonly string Text;
        public readonly string ProcessorKey;
        public readonly bool IsValid;

        public ProcessedTextCache(string text, string processorKey)
        {
            Text = text;
            ProcessorKey = processorKey;
            IsValid = true;
        }
    }
}
