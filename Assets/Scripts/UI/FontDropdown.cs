using TMPro;
using TypTyp;
using UnityEngine;
using UnityEngine.UI;

public class FontDropdown : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset[] fonts;
    [SerializeField] private GameObject optionsParent;
    private WritableButton writableButton;
    private TextMeshProUGUI label;
    private Image image;
    private string labelText;
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
        FontDropdownOption[] options = GetComponentsInChildren<FontDropdownOption>(true);
        for(int i = 0; i < options.Length; i++) options[i].Initialize(fonts[i], i);
        optionsParent.SetActive(false);
    }

    public void ToggleSelection()
    {
        optionsParent.SetActive(!optionsParent.activeSelf);
        label.text = optionsParent.activeSelf ? " - " : labelText;
        image.color = optionsParent.activeSelf ? Color.gray : Color.white;
        writableButton.OverrideText(label.text);
    }

    public void SetFont(int fontIdx, bool toggleSelection = true)
    {
        CurrentFontIdx = fontIdx;
        Settings.Instance.DefaultFont = fonts[fontIdx];
        labelText = fonts[fontIdx].name;
        if(toggleSelection) ToggleSelection();
        foreach (var config in FindObjectsByType<DefaultFontConfigurator>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            config.ResetFont();
    }
}
