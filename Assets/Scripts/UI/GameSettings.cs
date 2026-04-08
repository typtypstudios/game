using TypTyp;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour, INavigationLeaveReceiver
{
    [SerializeField] private Toggle showSpacesToggle;
    [SerializeField] private Toggle capLocksWarningToggle;
    [SerializeField] private Toggle filterChatToggle;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer mixer;
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

    public void ToggleShowSpaces() => showSpacesToggle.isOn = !showSpacesToggle.isOn;

    public void SetShowSpaces(bool value) => Settings.Instance.ShowSpaces = value;

    public void ToggleCapsWarning() => capLocksWarningToggle.isOn = !capLocksWarningToggle.isOn;

    public void SetCapsWarning(bool value) => Settings.Instance.CapsLockWarning = value;

    public void ToggleFilterChat() => filterChatToggle.isOn = !filterChatToggle.isOn;

    public void SetFilterChat(bool value) => Settings.Instance.ChatActive = value;
    
    public void AddVolume(float volumeToAdd) => volumeSlider.value += volumeToAdd;

    public void SetVolume(float value)
    {
        value = Mathf.Clamp01(value);
        float minDB = -80f;
        float maxDB = 20f;
        float logValue = Mathf.Log10(1f + value * 9f) / Mathf.Log10(10f);
        mixer.SetFloat("MasterVolume", Mathf.Lerp(minDB, maxDB, logValue));
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
            Debug.Log("Chat active: " + data.chatActive);
            filterChatToggle.isOn = data.chatActive;
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
    }

    public void OnLeave()
    {
        Save();
    }
}
