using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class WritableButton : AInputListener
{
    [SerializeField] private Color highlightColor = Color.yellow;
    private Button button;
    private TextMeshProUGUI buttonText;
    private int textLength;
    private string colorTag;
    private int idx;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        textLength = buttonText.text.Length;
        colorTag = $"<color #{ColorUtility.ToHtmlStringRGB(highlightColor)}>";
    }

    protected override void ProcessInput(char c)
    {
        buttonText.text = buttonText.text.Replace(colorTag, "").Replace("</color>", "");
        if (buttonText.text[idx] == c)
        {
            string original = buttonText.text;
            buttonText.text = colorTag + original[..(idx + 1)] + "</color>" + original[(idx + 1)..];
            if (++idx == textLength)
            {
                button.onClick?.Invoke();
                idx = 0;
            }
        }
        else idx = 0;
    }
}
