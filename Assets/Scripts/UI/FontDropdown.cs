using TMPro;
using TypTyp;
using UnityEngine;
using UnityEngine.UI;

public class FontDropdown : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset[] fonts;
    private WritableButton writableButton;
    private Image image;
    public FontDropdownOption[] Options { get; private set; }
    public FontDropdownOption ChosenOption { get; private set; }
    public int CurrentFontIdx { get; private set; } = 0;

    private void Awake()
    {
        writableButton = GetComponent<WritableButton>();
        image = GetComponent<Image>();
        Options = GetComponentsInChildren<FontDropdownOption>(true);
    }

    private void Start()
    {
        writableButton.OverrideText(Settings.Instance.DefaultFont.name);
        InitializeButtons();
    }

    //Los botones no se crean automáticamente, hay que asegurar que hay uno por fuente
    private void InitializeButtons()
    {
        for (int i = 0; i < Options.Length; i++)
        {
            Options[i].Initialize(fonts[i], i);
            Options[i].gameObject.SetActive(false);
        }
    }

    public void ToggleSelection()
    {
        foreach (var option in Options) option.ToggleActivation();
        writableButton.Block = true;
        image.color = Color.gray;
        writableButton.OverrideText(" - ");
    }

    public void DisplayFontInfo()
    {
        writableButton.Block = false;
        image.color = Color.white;
        writableButton.OverrideText(fonts[CurrentFontIdx].name);
    }

    public void SetFont(int fontIdx)
    {
        CurrentFontIdx = fontIdx;
        ChosenOption = Options[fontIdx];
        Settings.Instance.DefaultFont = fonts[fontIdx];
        foreach (var config in FindObjectsByType<DefaultFontConfigurator>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            config.ResetFont();
    }
}
