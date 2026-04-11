using System;
using UnityEngine;

namespace TypTyp.TextSystem.Typable
{
    [CreateAssetMenu(fileName = "TypableConfigPreset", menuName = "TypTyp/Typable/Typable Config Preset")]
    public class TypableConfigPreset : ScriptableObject
    {
        [field: SerializeField] public TypableConfig Config { get; set; }
        public event Action OnChange;

        void Reset() => Config = TypableConfig.Default;

        public void SetCaseSensitive(bool value)
        {
            TypableConfig config = Config;
            config.CaseSensitive = value;
            Config = config;
            OnChange?.Invoke();
        }
    }
}
