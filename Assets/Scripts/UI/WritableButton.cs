using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

[RequireComponent(typeof(Button))]
public class WritableButton : AInputListener
{
    [SerializeField] private bool resetIfFailed = true;
    [SerializeField] private bool resetOnWritten = true;
    [SerializeField] private float resetTime = 0.5f;
    private Button button;
    private TextMeshProUGUI buttonText;
    private string originalText;
    private int textLength;
    private int idx;
    private WaitForSeconds resetTimer;
    private Coroutine resetCoroutine;
    private Canvas canvas;
    public bool Block { get; set; } = false;
    private static event Action<WritableButton> OnButtonWritten;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        originalText = buttonText.text.Trim();
        textLength = originalText.Length;
        canvas = GetComponentInParent<Canvas>();
        resetTimer = new(resetTime);
        OnButtonWritten += OnOtherButtonWritten;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
        ResetButton();
    }

    private void OnOtherButtonWritten(WritableButton b)
    {
        if (b != this && resetIfFailed) ResetButton();
    }

    public void OverrideText(string text)
    {
        originalText = text;
        textLength = text.Length;
        ResetButton();
    }

    public void ResetButton(bool force = false)
    {
        if(!resetOnWritten && !force)
        {
            Block = true;
            return;
        }
        buttonText.text = originalText;
        idx = 0;
        resetCoroutine = null;
    }

    protected override void ProcessInput(char c)
    {
        if(!canvas.enabled || Block) return;
        if (originalText[idx].ToString().ToLower().Equals(c.ToString().ToLower()))
        {
            if(resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
                ResetButton();
            }
            buttonText.text = fillColorTag + originalText[..(idx + 1)] + "</color>" + 
                originalText[(idx + 1)..];
            if (++idx == textLength)
            {
                resetCoroutine = StartCoroutine(ResetButtonCoroutine());
                button.onClick?.Invoke();
                OnButtonWritten?.Invoke(this);
            }
        }
        else if(resetIfFailed) ResetButton();
    }

    public void CompletelyBlock(bool block)
    {
        Block = block;
        button.interactable = !block;
    }

    IEnumerator ResetButtonCoroutine()
    {
        idx = 0;
        yield return resetTimer;
        ResetButton();
    }
}
