using TMPro;
using TypTyp;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FontDropdown : MonoBehaviour
{
    [SerializeField] private InputActionReference clickReference;
    [SerializeField] private TMP_FontAsset[] fonts;
    private WritableButton writableButton;
    private Image image;
    private Color originalColor;
    private bool isDeployed = false;
    public FontDropdownOption[] Options { get; private set; }
    public FontDropdownOption ChosenOption { get; private set; }
    public int CurrentFontIdx { get; private set; } = 0;

    private void Awake()
    {
        writableButton = GetComponent<WritableButton>();
        image = GetComponent<Image>();
        originalColor = image.color;
        Options = GetComponentsInChildren<FontDropdownOption>(true);
        clickReference.action.canceled += HandleClick;
    }

    private void Start()
    {
        writableButton.OverrideText(Settings.Instance.DefaultFont.name);
        InitializeButtons();
    }

    private void OnDestroy()
    {
        clickReference.action.canceled -= HandleClick;
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
        isDeployed = !isDeployed;
        foreach (var option in Options) option.ToggleActivation();
        writableButton.Block = true;
        image.color = originalColor * 0.5f;
        writableButton.OverrideText(" - ");
    }

    public void DisplayFontInfo()
    {
        writableButton.Block = false;
        image.color = originalColor;
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

    private void HandleClick(InputAction.CallbackContext _)
    {
        if (isDeployed) ToggleSelection();
    }
}
