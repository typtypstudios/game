using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Button))]
public class WritableButton : AInputListener
{
    [SerializeField] private bool resetIfFailed = true;
    private Button button;
    private TextMeshProUGUI buttonText;
    private string originalText;
    private int textLength;
    private int idx;
    private readonly WaitForSeconds resetTimer = new(0.5f);
    private Coroutine resetCoroutine;
    private Canvas canvas;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        originalText = buttonText.text.Trim();
        textLength = originalText.Length;
        canvas = GetComponentInParent<Canvas>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
        ResetButton();
    }

    public void OverrideText(string text)
    {
        originalText = text;
        textLength = text.Length;
        ResetButton();
    }

    private void ResetButton()
    {
        buttonText.text = originalText;
        idx = 0;
        resetCoroutine = null;
    }

    protected override void ProcessInput(char c)
    {
        if(!canvas.enabled) return;
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
            }
        }
        else if(resetIfFailed) ResetButton();
    }

    IEnumerator ResetButtonCoroutine()
    {
        idx = 0;
        yield return resetTimer;
        ResetButton();
    }
}
