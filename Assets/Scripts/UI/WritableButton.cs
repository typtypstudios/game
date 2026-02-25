using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class WritableButton : AInputListener
{
    [SerializeField] private bool resetIfFailed = true;
    private Button button;
    private TextMeshProUGUI buttonText;
    private string originalText;
    private int textLength;
    private int idx;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        textLength = buttonText.text.Length;
        originalText = buttonText.text;
    }

    private void ResetButton()
    {
        buttonText.text = originalText;
        idx = 0;
    }

    protected override void ProcessInput(char c)
    {
        if (originalText[idx] == c)
        {
            buttonText.text = buttonText.text.Replace(fillColorTag, "").Replace("</color>", "");
            buttonText.text = fillColorTag + originalText[..(idx + 1)] + "</color>" + originalText[(idx + 1)..];
            if (++idx == textLength)
            {
                button.onClick?.Invoke();
                ResetButton();
            }
        }
        else if(resetIfFailed) ResetButton();
    }
}
