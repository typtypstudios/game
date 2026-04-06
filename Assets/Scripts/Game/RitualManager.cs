using UnityEngine;
using System;
using TypTyp.TextSystem;
using TypTyp.TextSystem.Typable;
using TypTyp;

public class RitualManager : MonoBehaviour
{
    private TypableController typableController;
    private ITextProvider textProvider;
    private int numTextsCompleted = 0;
    private int lastIdx = 0;
    private bool lastHasMistake = false;
    private string originalText = "";

    public string OriginalText
    {
        get => originalText;
        set => SetText(value);
    }

    public event Action OnCorrectChar;
    public event Action OnWrongChar;
    public event Action<float> OnProgressUpdated;
    public event Action<int> LineCompleted;

    void Awake()
    {
        typableController = GetComponent<TypableController>();
        textProvider = GetComponentInParent<ITextProvider>();
        UnityEngine.Assertions.Assert.IsNotNull(typableController);
        if (typableController != null)
        {
            typableController.InputTransform = TransformInput;
        }
    }

    void OnEnable()
    {
        if (typableController == null) return;
        typableController.OnChanged += HandleChanged;
        typableController.OnError += HandleError;
        typableController.OnComplete += HandleComplete;
    }

    void OnDisable()
    {
        if (typableController == null) return;
        typableController.OnChanged -= HandleChanged;
        typableController.OnError -= HandleError;
        typableController.OnComplete -= HandleComplete;
    }

    public void SetText(string text)
    {
        originalText = text ?? "";
        lastIdx = 0;
        lastHasMistake = false;
        if (typableController != null)
            typableController.SetText(originalText);
        UpdateProgress();
    }

    private void HandleChanged()
    {
        int idx = typableController.Idx;
        if (idx > lastIdx)
        {
            OnCorrectChar?.Invoke();
        }

        lastIdx = idx;
        lastHasMistake = typableController.HasMistake;
        UpdateProgress();
    }

    private void HandleError()
    {
        if (!lastHasMistake)
        {
            lastHasMistake = true;
            OnWrongChar?.Invoke();
        }
    }

    private void HandleComplete()
    {
        numTextsCompleted++;
        GetNextText();
    }

    private void GetNextText()
    {
        if (textProvider == null) return;
        SetText(textProvider.GetNextText());
        //Esto solo ocurre en cliente
        LineCompleted?.Invoke(numTextsCompleted);
    }

    private void UpdateProgress()
    {
        if (typableController == null) return;
        float globalProgress = (float)numTextsCompleted / TypTyp.Settings.Instance.MaxTextsProvided;
        float localProgress = originalText.Length == 0 ? 0 :
            (float)typableController.Idx / (originalText.Length * TypTyp.Settings.Instance.MaxTextsProvided);
        float progress = globalProgress + localProgress;
        OnProgressUpdated?.Invoke(progress);
    }

    private char TransformInput(char c)
    {
        if (Settings.Instance.ShowSpaces && c == ' ') return '-';
        return c;
    }
}
