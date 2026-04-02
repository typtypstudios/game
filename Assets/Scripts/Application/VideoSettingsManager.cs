using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TypTyp.Application
{
    public static class VideoSettingsManager
    {
        private static VideoSettingsData currentSettings;
        private static bool hasCurrentSettings;

        public static VideoSettingsData GetDefaultSettings()
        {
            return CreateNormalizedDefaultSettings();
        }

        public static VideoSettingsData GetCurrentSettings()
        {
            if (!hasCurrentSettings)
            {
                currentSettings = CreateNormalizedDefaultSettings();
                hasCurrentSettings = true;
            }

            return currentSettings;
        }

        public static void SetCurrentSettings(VideoSettingsData settings)
        {
            currentSettings = Normalize(settings);
            hasCurrentSettings = true;
        }

        public static void ApplyCurrentSettings()
        {
            ApplySettings(GetCurrentSettings());
        }

        public static IReadOnlyList<VideoSettingsData> GetAvailableResolutions()
        {
            Resolution[] resolutions = Screen.resolutions;
            if (resolutions == null || resolutions.Length == 0)
            {
                return new[] { CreateSettingsFromResolution(Screen.currentResolution, Screen.fullScreenMode, true, QualitySettings.GetQualityLevel()) };
            }

            return resolutions
                .Select(resolution => CreateSettingsFromResolution(resolution, Screen.fullScreenMode, true, QualitySettings.GetQualityLevel()))
                .GroupBy(CreateResolutionKey)
                .Select(group => group.OrderByDescending(option => option.RefreshRateHz).First())
                .OrderByDescending(option => option.ResolutionWidth)
                .ThenByDescending(option => option.ResolutionHeight)
                .ThenByDescending(option => option.RefreshRateHz)
                .ToArray();
        }

        public static IReadOnlyList<FullScreenMode> GetSupportedFullScreenModes()
        {
            return Enum.GetValues(typeof(FullScreenMode)).Cast<FullScreenMode>().ToArray();
        }

        public static void ApplySettings(VideoSettingsData settings)
        {
            VideoSettingsData normalized = Normalize(settings);
            currentSettings = normalized;
            hasCurrentSettings = true;

            QualitySettings.SetQualityLevel(normalized.QualityLevel, true);
            QualitySettings.vSyncCount = normalized.VSyncEnabled ? 1 : 0;

            if (normalized.VSyncEnabled)
            {
                UnityEngine.Application.targetFrameRate = -1;
            }
            else
            {
                UnityEngine.Application.targetFrameRate = Mathf.Max(30, normalized.TargetFrameRate);
            }

            Screen.SetResolution(
                normalized.ResolutionWidth,
                normalized.ResolutionHeight,
                normalized.FullScreenMode,
                CreateRefreshRate(normalized));
        }

        public static VideoSettingsData UpdateFrameRate(VideoSettingsData settings, int newFrameRate)
        {
            VideoSettingsData updated = settings;
            updated.TargetFrameRate = newFrameRate;
            return Normalize(updated);
        }

        private static VideoSettingsData CreateNormalizedDefaultSettings()
        {
            Resolution currentResolution = Screen.currentResolution;
            VideoSettingsData defaults = CreateSettingsFromResolution(
                currentResolution,
                Screen.fullScreenMode,
                true,
                QualitySettings.GetQualityLevel());

            defaults.TargetFrameRate = Mathf.Min(60, defaults.RefreshRateHz);
            return Normalize(defaults);
        }

        private static VideoSettingsData CreateSettingsFromResolution(Resolution resolution, FullScreenMode fullScreenMode, bool vSyncEnabled, int qualityLevel)
        {
            int refreshRateNumerator = (int)resolution.refreshRateRatio.numerator;
            int refreshRateDenominator = (int)Math.Max(1u, resolution.refreshRateRatio.denominator);
            int refreshRateHz = Mathf.Max(1, Mathf.RoundToInt((float)refreshRateNumerator / refreshRateDenominator));

            return new VideoSettingsData(
                targetFrameRate: refreshRateHz,
                vSyncEnabled: vSyncEnabled,
                resolutionWidth: resolution.width,
                resolutionHeight: resolution.height,
                refreshRateNumerator: refreshRateNumerator,
                refreshRateDenominator: refreshRateDenominator,
                fullScreenMode: fullScreenMode,
                qualityLevel: qualityLevel);
        }

        private static VideoSettingsData Normalize(VideoSettingsData settings)
        {
            Resolution resolvedResolution = ResolveResolution(settings);

            VideoSettingsData normalized = new VideoSettingsData(
                targetFrameRate: settings.TargetFrameRate,
                vSyncEnabled: settings.VSyncEnabled,
                resolutionWidth: resolvedResolution.width,
                resolutionHeight: resolvedResolution.height,
                refreshRateNumerator: (int)resolvedResolution.refreshRateRatio.numerator,
                refreshRateDenominator: (int)Math.Max(1u, resolvedResolution.refreshRateRatio.denominator),
                fullScreenMode: NormalizeFullScreenMode(settings.FullScreenMode),
                qualityLevel: NormalizeQualityLevel(settings.QualityLevel));

            int maxFrameRate = normalized.RefreshRateHz;
            normalized.TargetFrameRate = Mathf.Clamp(settings.TargetFrameRate <= 0 ? maxFrameRate : settings.TargetFrameRate, 30, maxFrameRate);
            return normalized;
        }

        private static Resolution ResolveResolution(VideoSettingsData settings)
        {
            Resolution[] resolutions = Screen.resolutions;
            Resolution currentResolution = Screen.currentResolution;

            if (resolutions == null || resolutions.Length == 0)
            {
                return currentResolution;
            }

            foreach (Resolution resolution in resolutions)
            {
                if (MatchesResolution(resolution, settings))
                {
                    return resolution;
                }
            }

            Resolution sameSize = resolutions
                .Where(resolution => resolution.width == settings.ResolutionWidth && resolution.height == settings.ResolutionHeight)
                .OrderByDescending(resolution => resolution.refreshRateRatio.value)
                .FirstOrDefault();

            if (sameSize.width > 0 && sameSize.height > 0)
            {
                return sameSize;
            }

            return currentResolution;
        }

        private static bool MatchesResolution(Resolution resolution, VideoSettingsData settings)
        {
            return resolution.width == settings.ResolutionWidth
                && resolution.height == settings.ResolutionHeight
                && (int)resolution.refreshRateRatio.numerator == settings.RefreshRateNumerator
                && (int)resolution.refreshRateRatio.denominator == Mathf.Max(1, settings.RefreshRateDenominator);
        }

        private static string CreateResolutionKey(VideoSettingsData settings)
        {
            return $"{settings.ResolutionWidth}x{settings.ResolutionHeight}@{settings.RefreshRateNumerator}/{settings.RefreshRateDenominator}";
        }

        private static int NormalizeQualityLevel(int qualityLevel)
        {
            int maxQualityIndex = Mathf.Max(0, QualitySettings.names.Length - 1);
            return Mathf.Clamp(qualityLevel, 0, maxQualityIndex);
        }

        private static FullScreenMode NormalizeFullScreenMode(FullScreenMode fullScreenMode)
        {
            return Enum.IsDefined(typeof(FullScreenMode), fullScreenMode)
                ? fullScreenMode
                : FullScreenMode.FullScreenWindow;
        }

        private static RefreshRate CreateRefreshRate(VideoSettingsData settings)
        {
            return new RefreshRate
            {
                numerator = (uint)Mathf.Max(1, settings.RefreshRateNumerator),
                denominator = (uint)Mathf.Max(1, settings.RefreshRateDenominator),
            };
        }
    }
}
