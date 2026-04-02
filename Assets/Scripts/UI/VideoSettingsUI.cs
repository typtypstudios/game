using System.Collections.Generic;
using System.Linq;
using TMPro;
using TypTyp.Application;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettingsUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown fullScreenModeDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle vSyncToggle;

    private List<VideoSettingsData> resolutionOptions = new();
    private List<FullScreenMode> fullScreenModeOptions = new();
    private VideoSettingsData editedSettings;

    private void OnEnable()
    {
        SaveManager.Instance.OnBeforeSave += HandleBeforeSave;
        SaveManager.Instance.OnAfterLoad += HandleAfterLoad;
    }

    private void Start()
    {
        RefreshOptions();
        RefreshFromManager();
    }

    private void OnDisable()
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        SaveManager.Instance.OnBeforeSave -= HandleBeforeSave;
        SaveManager.Instance.OnAfterLoad -= HandleAfterLoad;
    }

    public void SetResolutionIndex(int optionIndex)
    {
        if (optionIndex < 0 || optionIndex >= resolutionOptions.Count)
        {
            return;
        }

        VideoSettingsData selectedResolution = resolutionOptions[optionIndex];
        editedSettings.ResolutionWidth = selectedResolution.ResolutionWidth;
        editedSettings.ResolutionHeight = selectedResolution.ResolutionHeight;
        editedSettings.RefreshRateNumerator = selectedResolution.RefreshRateNumerator;
        editedSettings.RefreshRateDenominator = selectedResolution.RefreshRateDenominator;
        editedSettings.TargetFrameRate = selectedResolution.RefreshRateHz;
    }

    public void SetFullScreenMode(int optionIndex)
    {
        if (optionIndex < 0 || optionIndex >= fullScreenModeOptions.Count)
        {
            return;
        }

        editedSettings.FullScreenMode = fullScreenModeOptions[optionIndex];
    }

    public void SetVSync(bool enabled)
    {
        editedSettings.VSyncEnabled = enabled;
    }

    public void SetQualityLevel(int optionIndex)
    {
        editedSettings.QualityLevel = optionIndex;
    }

    public void Apply()
    {
        VideoSettingsManager.ApplySettings(editedSettings);
        RefreshFromManager();
    }

    public void Save()
    {
        VideoSettingsManager.SetCurrentSettings(editedSettings);
        SaveManager.Instance.Save();
        VideoSettingsManager.ApplyCurrentSettings();
        RefreshFromManager();
    }

    public void Revert()
    {
        RefreshFromManager();
    }

    private void HandleAfterLoad(SaveState state)
    {
        RefreshOptions();
        RefreshFromManager();
    }

    private void HandleBeforeSave(SaveState state)
    {
        VideoSettingsManager.SetCurrentSettings(editedSettings);
        state.global.videoSettings = VideoSettingsManager.GetCurrentSettings();
    }

    private void RefreshOptions()
    {
        resolutionOptions = VideoSettingsManager.GetAvailableResolutions().ToList();
        fullScreenModeOptions = VideoSettingsManager.GetSupportedFullScreenModes().ToList();

        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(resolutionOptions.Select(FormatResolutionLabel).ToList());
        }

        if (fullScreenModeDropdown != null)
        {
            fullScreenModeDropdown.ClearOptions();
            fullScreenModeDropdown.AddOptions(fullScreenModeOptions.Select(mode => mode.ToString()).ToList());
        }

        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(QualitySettings.names.ToList());
        }
    }

    private void RefreshFromManager()
    {
        editedSettings = VideoSettingsManager.GetCurrentSettings();
        SyncWidgets();
    }

    private void SyncWidgets()
    {
        if (resolutionDropdown != null)
        {
            resolutionDropdown.SetValueWithoutNotify(FindResolutionIndex(editedSettings));
        }

        if (fullScreenModeDropdown != null)
        {
            fullScreenModeDropdown.SetValueWithoutNotify(Mathf.Max(0, fullScreenModeOptions.IndexOf(editedSettings.FullScreenMode)));
        }

        if (qualityDropdown != null)
        {
            qualityDropdown.SetValueWithoutNotify(Mathf.Clamp(editedSettings.QualityLevel, 0, Mathf.Max(0, QualitySettings.names.Length - 1)));
        }

        if (vSyncToggle != null)
        {
            vSyncToggle.SetIsOnWithoutNotify(editedSettings.VSyncEnabled);
        }
    }

    private int FindResolutionIndex(VideoSettingsData settings)
    {
        int index = resolutionOptions.FindIndex(option =>
            option.ResolutionWidth == settings.ResolutionWidth &&
            option.ResolutionHeight == settings.ResolutionHeight &&
            option.RefreshRateNumerator == settings.RefreshRateNumerator &&
            option.RefreshRateDenominator == settings.RefreshRateDenominator);

        return Mathf.Max(0, index);
    }

    private static string FormatResolutionLabel(VideoSettingsData settings)
    {
        return $"{settings.ResolutionWidth} x {settings.ResolutionHeight} @ {settings.RefreshRateHz}Hz";
    }
}
