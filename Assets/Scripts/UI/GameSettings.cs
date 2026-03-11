using TypTyp;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private Toggle showSpacesToggle;
    [SerializeField] private Toggle capLocksWarningToggle;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer mixer;
    private FontDropdown fontDropdown;

    private void Start()
    {
        fontDropdown = GetComponentInChildren<FontDropdown>();
        showSpacesToggle.isOn = PlayerPrefs.GetInt("ShowSpaces", 1) == 1;
        SetShowSpaces(showSpacesToggle.isOn);
        capLocksWarningToggle.isOn = PlayerPrefs.GetInt("CapLocksWarnigng", 0) == 1;
        SetCapsWarning(capLocksWarningToggle.isOn);
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.75f);
        SetVolume(volumeSlider.value);
        fontDropdown.SetFont(PlayerPrefs.GetInt("Font", 0));
    }

    public void Save()
    {
        PlayerPrefs.SetInt("ShowSpaces", showSpacesToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("CapLocksWarnigng", capLocksWarningToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetInt("Font", fontDropdown.CurrentFontIdx);
    }

    public void ToggleShowSpaces() => showSpacesToggle.isOn = !showSpacesToggle.isOn;

    public void SetShowSpaces(bool value) => Settings.Instance.ShowSpaces = value;

    public void ToggleCapsWarning() => capLocksWarningToggle.isOn = !capLocksWarningToggle.isOn;

    public void SetCapsWarning(bool value) => Settings.Instance.CapsLockWarning = value;

    public void AddVolume(float volumeToAdd) => volumeSlider.value += volumeToAdd;

    public void SetVolume(float value)
    {
        value = Mathf.Clamp01(value);
        float minDB = -80f;
        float maxDB = 20f;
        float logValue = Mathf.Log10(1f + value * 9f) / Mathf.Log10(10f);
        mixer.SetFloat("MasterVolume", Mathf.Lerp(minDB, maxDB, logValue));
    }
}
