using UnityEngine;

namespace TypTyp.TextSystem.Typable
{
    [CreateAssetMenu(fileName = "TypableViewStylePreset", menuName = "TypTyp/Typable/Typable Style Preset")]
    public class TypableViewStylePreset : ScriptableObject
    {
        [field: SerializeField] public TypableViewStyleConfig Config { get; private set; }

        void Reset() => Config = TypableViewStyleConfig.Default;
    }
}
