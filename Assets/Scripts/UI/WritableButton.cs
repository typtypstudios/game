using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using TypTyp;
using TypTyp.TextSystem.Typable;

[RequireComponent(typeof(Button))]
public class WritableButton : MonoBehaviour
{
    [SerializeField] private bool resetIfFailed = true;
    [SerializeField] private bool resetOnWritten = true;
    [SerializeField] private float resetTime = 0.5f;
    [SerializeField] private TypableController typableController;
    [SerializeField] private TMPTypableView tmpView;
    private Button button;
    private string originalText;
    private WaitForSeconds resetTimer;
    private Coroutine resetCoroutine;
    private Canvas parentCanvas;
    public bool Block { get; set; } = false;
    private static event Action<WritableButton> OnButtonWritten;

    private void Awake()
    {
        button = GetComponent<Button>();
        var tmp = button.GetComponentInChildren<TextMeshProUGUI>();
        originalText = tmp != null ? tmp.text.Trim() : string.Empty;
        parentCanvas = GetComponentInParent<Canvas>();
        resetTimer = new(resetTime);
        OnButtonWritten += OnOtherButtonWritten;
        if (typableController == null)
            typableController = GetComponent<TypableController>();
        if (tmpView == null)
            tmpView = GetComponent<TMPTypableView>();
    }

    private void OnEnable()
    {
        if (typableController != null)
        {
            typableController.OnComplete += HandleComplete;
            typableController.OnError += HandleError;
        }

        if (typableController != null && !string.IsNullOrEmpty(originalText))
            typableController.SetText(originalText);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (typableController != null)
        {
            typableController.OnComplete -= HandleComplete;
            typableController.OnError -= HandleError;
        }
        ResetButton();
    }

    private bool InteractionEnabled()
    {
        bool canvasEnabled = parentCanvas.enabled;
        //Algunos canvas group son asignados en tiempo de ejecución
        CanvasGroup parentCanvasGroup = GetComponentInParent<CanvasGroup>();
        bool groupEnabled = parentCanvasGroup == null || 
            parentCanvasGroup.blocksRaycasts && parentCanvasGroup.interactable;
        return !Block && canvasEnabled && groupEnabled;
    }

    private void OnOtherButtonWritten(WritableButton b)
    {
        if (b != this && resetIfFailed) ResetButton();
    }

    public void OverrideText(string text)
    {
        originalText = text;
        ResetButton();
    }

    public void ResetButton(bool force = false)
    {
        if (!resetOnWritten && !force)
        {
            Block = true;
            if (typableController != null)
                typableController.enabled = false;
            return;
        }
        if (typableController != null)
        {
            typableController.enabled = true;
            typableController.SetText(originalText);
        }
        resetCoroutine = null;
    }

    private void HandleComplete()
    {
        if(!InteractionEnabled()) return;

        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            ResetButton();
        }

        resetCoroutine = StartCoroutine(ResetButtonCoroutine());
        button.onClick?.Invoke();
        OnButtonWritten?.Invoke(this);
    }

    private void HandleError()
    {
        if (!InteractionEnabled()) return;
        if (resetIfFailed) ResetButton();
    }

    public void CompletelyBlock(bool block)
    {
        Block = block;
        button.interactable = !block;
        if (typableController != null)
            typableController.enabled = !block;
    }

    public Color GetButtonColor()
    {
        if (tmpView != null)
            return tmpView.StyleConfig.CorrectColor;
        return Color.white;
    }

    IEnumerator ResetButtonCoroutine()
    {
        yield return resetTimer;
        ResetButton();
    }
}
