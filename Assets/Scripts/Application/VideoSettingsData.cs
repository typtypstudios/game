using System;
using UnityEngine;

namespace TypTyp.Application
{
    [Serializable]
    public struct VideoSettingsData
    {
        [field: SerializeField] public int TargetFrameRate { get; set; }
        [field: SerializeField] public bool VSyncEnabled { get; set; }
        [field: SerializeField] public int ResolutionWidth { get; set; }
        [field: SerializeField] public int ResolutionHeight { get; set; }
        [field: SerializeField] public int RefreshRateNumerator { get; set; }
        [field: SerializeField] public int RefreshRateDenominator { get; set; }
        [field: SerializeField] public FullScreenMode FullScreenMode { get; set; }
        [field: SerializeField] public int QualityLevel { get; set; }

        public readonly bool HasValidResolution => ResolutionWidth > 0 && ResolutionHeight > 0;
        public readonly int RefreshRateHz => Mathf.Max(1, Mathf.RoundToInt((float)Mathf.Max(1, RefreshRateNumerator) / Mathf.Max(1, RefreshRateDenominator)));

        public VideoSettingsData(int targetFrameRate = 60, bool vSyncEnabled = true, int resolutionWidth = 1920, int resolutionHeight = 1080, int refreshRateNumerator = 60, int refreshRateDenominator = 1, FullScreenMode fullScreenMode = FullScreenMode.FullScreenWindow, int qualityLevel = 2)
        {
            TargetFrameRate = targetFrameRate;
            VSyncEnabled = vSyncEnabled;
            ResolutionWidth = resolutionWidth;
            ResolutionHeight = resolutionHeight;
            RefreshRateNumerator = refreshRateNumerator;
            RefreshRateDenominator = Mathf.Max(1, refreshRateDenominator);
            FullScreenMode = fullScreenMode;
            QualityLevel = qualityLevel;
        }

        public static VideoSettingsData CreateDefault()
        {
            return new VideoSettingsData();
        }
    }
}
