using TypTyp;
using TypTyp.TextSystem.Typable;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour, INavigationLeaveReceiver
{
    [SerializeField] private TypableConfigPreset menuPreset;
    [SerializeField] private Toggle showSpacesToggle;
    [SerializeField] private Toggle capLocksWarningToggle;
    [SerializeField] private Toggle filterChatToggle;
    [SerializeField] private Toggle ignoreCaseToggle;
    [SerializeField] private Toggle largeTextToggle;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private Toggle antialiasingToggle;
    [SerializeField] private Slider volumeSlider;
    private FontDropdown fontDropdown;

    private void Awake()
    {
        fontDropdown = GetComponentInChildren<FontDropdown>();
    }

    private void OnEnable()
    {
        SaveManager.Instance.OnBeforeSave += HandleBeforeSave;
        SaveManager.Instance.OnAfterLoad += HandleAfterLoad;
    }

    private void Start()
    {
        if (SaveManager.Instance.HasLoadedState)
        {
            SaveState state = SaveManager.Instance.GetState();
            ApplySettings(state);
        }
        else
        {
            ApplySettings(new SaveState());
        }
    }

    private void OnDisable()
    {
        if (SaveManager.Instance == null) return;

        SaveManager.Instance.OnBeforeSave -= HandleBeforeSave;
        SaveManager.Instance.OnAfterLoad -= HandleAfterLoad;
    }

    public void Save()
    {
        SaveManager.Instance.Save();
    }

    public void SetShowSpaces(bool value) => Settings.Instance.ShowSpaces = value;

    public void SetCapsWarning(bool value) => Settings.Instance.CapsLockWarning = value;

    public void SetFilterChat(bool value) => Settings.Instance.ChatActive = value;

    public void SetIgnoreCase(bool value) => menuPreset.SetCaseSensitive(!value);

    public void SetLargeText(bool value) => Settings.Instance.LargeText = value;

    public void SetVolume(float value)
    {
        AudioManager.Instance.SetBusVolume("MasterVolume", Mathf.Clamp01(value));
    }

    public void SetVsync(bool value) => QualitySettings.vSyncCount = value ? 1 : 0;

    public void SetAntialiasing(bool value) 
    {
        var renderAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        renderAsset.msaaSampleCount = value ? 4 : 0;
        GraphicsSettings.defaultRenderPipeline = GraphicsSettings.defaultRenderPipeline;
    }

    private void HandleBeforeSave(SaveState state)
    {
        if (showSpacesToggle != null)
        {
            state.global.showSpaces = showSpacesToggle.isOn;
        }

        if (filterChatToggle != null)
        {
            state.global.chatActive = filterChatToggle.isOn;
        }

        if (ignoreCaseToggle != null)
        {
            state.global.ignoreCaseMenus = ignoreCaseToggle.isOn;
        }

        if (largeTextToggle != null)
        {
            state.global.largeText = largeTextToggle.isOn;
        }

        if (capLocksWarningToggle != null)
        {
            state.global.capsLockWarning = capLocksWarningToggle.isOn;
        }

        if (volumeSlider != null)
        {
            state.global.volume = volumeSlider.value;
        }

        if (fontDropdown != null)
        {
            state.global.fontIndex = fontDropdown.CurrentFontIdx;
        }

        if (vsyncToggle != null)
        {
            state.global.vsync = vsyncToggle.isOn;
        }

        if (antialiasingToggle != null)
        {
            state.global.antialiasing = antialiasingToggle.isOn;
        }
    }

    private void HandleAfterLoad(SaveState state)
    {
        ApplySettings(state);
    }

    private void ApplySettings(SaveState state)
    {
        GlobalSettingsData data = state.global ?? new GlobalSettingsData();

        SetShowSpaces(data.showSpaces);
        if (showSpacesToggle != null)
        {
            showSpacesToggle.isOn = data.showSpaces;
        }

        SetFilterChat(data.chatActive);
        if (filterChatToggle != null)
        {
            filterChatToggle.isOn = data.chatActive;
        }

        SetIgnoreCase(data.ignoreCaseMenus);
        if (ignoreCaseToggle != null)
        {
            ignoreCaseToggle.isOn = data.ignoreCaseMenus;
        }

        SetLargeText(data.largeText);
        if (largeTextToggle != null)
        {
            largeTextToggle.isOn = data.largeText;
        }

        SetCapsWarning(data.capsLockWarning);
        if (capLocksWarningToggle != null)
        {
            capLocksWarningToggle.isOn = data.capsLockWarning;
        }

        SetVolume(data.volume);
        if (volumeSlider != null)
        {
            volumeSlider.value = data.volume;
        }

        if (fontDropdown != null)
        {
            if (fontDropdown.Options != null && fontDropdown.Options.Length > 0)
            {
                int safeFontIndex = Mathf.Clamp(data.fontIndex, 0, fontDropdown.Options.Length - 1);
                fontDropdown.SetFont(safeFontIndex);
            }
        }

        SetVsync(data.vsync);
        if (vsyncToggle != null)
        {
            vsyncToggle.isOn = data.vsync;
        }

        SetAntialiasing(data.antialiasing);
        if (antialiasingToggle != null)
        {
            antialiasingToggle.isOn = data.antialiasing;
        }
    }

    public void OnLeave()
    {
        Save();
    }
}
