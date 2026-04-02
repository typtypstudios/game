using TypTyp;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
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

    public void SetFilterChat(bool value)
    {
        Settings.Instance.ChatActive = value;
    }

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
        state.global.showSpaces = showSpacesToggle.isOn;
        state.global.chatActive = filterChatToggle.isOn;
        state.global.capsLockWarning = capLocksWarningToggle.isOn;
        state.global.volume = volumeSlider.value;
        state.global.fontIndex = fontDropdown.CurrentFontIdx;
    }

    private void HandleAfterLoad(SaveState state)
    {
        ApplySettings(state);
    }

    private void ApplySettings(SaveState state)
    {
        GlobalSettingsData data = state.global ?? new GlobalSettingsData();

        showSpacesToggle.isOn = data.showSpaces;
        SetShowSpaces(data.showSpaces);

        filterChatToggle.isOn = data.chatActive;
        SetFilterChat(data.chatActive);

        capLocksWarningToggle.isOn = data.capsLockWarning;
        SetCapsWarning(data.capsLockWarning);

        volumeSlider.value = data.volume;
        SetVolume(data.volume);

        fontDropdown.SetFont(data.fontIndex);
    }
}
