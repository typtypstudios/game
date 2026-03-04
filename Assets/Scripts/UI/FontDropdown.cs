using TMPro;
using TypTyp;
using UnityEngine;
using UnityEngine.UI;

public class FontDropdown : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset[] fonts;
    public FontDropdownOption[] Options { get; private set; }
    private WritableButton writableButton;
    private TextMeshProUGUI label;
    private Image image;
    private string labelText;
    private bool selected = false;
    public int CurrentFontIdx { get; private set; } = 0;

    private void Start()
    {
        writableButton = GetComponent<WritableButton>();
        label = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<Image>();
        writableButton.OverrideText(Settings.Instance.DefaultFont.name);
        labelText = label.text;
        InitializeButtons();
    }

    //Los botones no se crean automáticamente, hay que asegurar que hay uno por fuente
    private void InitializeButtons()
    {
        Options = GetComponentsInChildren<FontDropdownOption>(true);
        for (int i = 0; i < Options.Length; i++)
        {
            Options[i].Initialize(fonts[i], i);
            Options[i].gameObject.SetActive(false);
        }
    }

    public void ToggleSelection()
    {
        selected = !selected;
        foreach (var option in Options) option.gameObject.SetActive(selected);
        label.text = selected ? " - " : labelText;
        image.color = selected ? Color.gray : Color.white;
        writableButton.OverrideText(label.text);
        writableButton.Block = selected;
    }

    public void SetFont(int fontIdx)
    {
        CurrentFontIdx = fontIdx;
        Settings.Instance.DefaultFont = fonts[fontIdx];
        labelText = fonts[fontIdx].name;
        foreach (var config in FindObjectsByType<DefaultFontConfigurator>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            config.ResetFont();
    }
}
