using UnityEngine;

namespace TypTyp.TextSystem.Typable
{
    [CreateAssetMenu(fileName = "TypableConfigPreset", menuName = "TypTyp/Typable/Typable Config Preset")]
    public class TypableConfigPreset : ScriptableObject
    {
        [field: SerializeField] public TypableConfig Config { get; private set; }

        void Reset() => Config = TypableConfig.Default;
    }
}
