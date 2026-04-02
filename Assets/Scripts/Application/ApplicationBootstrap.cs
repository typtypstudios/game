using UnityEngine;

namespace TypTyp.Application
{
    internal static class ApplicationBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            SaveManager saveManager = SaveManager.Instance;
            saveManager.OnAfterLoad -= HandleAfterLoad;
            saveManager.OnAfterLoad += HandleAfterLoad;

            if (saveManager.HasLoadedState)
            {
                var state = saveManager.GetState();
                ApplySavedOrDefaultVideoSettings(state);
            }
        }

        private static void HandleAfterLoad(SaveState state)
        {
            ApplySavedOrDefaultVideoSettings(state);
        }

        private static void ApplySavedOrDefaultVideoSettings(SaveState state)
        {
            VideoSettingsData settings = state?.global?.videoSettings ?? VideoSettingsManager.GetDefaultSettings();
            VideoSettingsManager.SetCurrentSettings(settings);
            VideoSettingsManager.ApplyCurrentSettings();
        }
    }
}
