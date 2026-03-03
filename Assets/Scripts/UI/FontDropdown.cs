using TMPro;
using TypTyp;
using UnityEngine;
using UnityEngine.UI;

public class FontDropdown : MonoBehaviour
{
    [SerializeField] private GameObject options;
    private WritableButton writableButton;
    private TextMeshProUGUI label;
    private Image image;
    private string labelText;

    private void Start()
    {
        writableButton = GetComponent<WritableButton>();
        label = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<Image>();
        writableButton.OverrideText(Settings.Instance.DefaultFont.name);
        labelText = label.text;
    }

    public void ToggleSelection()
    {
        options.SetActive(!options.activeSelf);
        label.text = options.activeSelf ? " - " : labelText;
        image.color = options.activeSelf ? Color.gray : Color.white;
        writableButton.OverrideText(label.text);
    }

    public void SetFont(TMP_FontAsset font)
    {
        Settings.Instance.DefaultFont = font;
        labelText = font.name;
        ToggleSelection();
        foreach (var config in FindObjectsByType<DefaultFontConfigurator>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            config.ResetFont();
    }
}
