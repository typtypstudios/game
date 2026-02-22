using UnityEngine;

namespace TypTyp
{
    [CreateAssetMenu(menuName = "TypTyp/Settings")]
    public class Settings : ScriptableSingleton<Settings>
    {
        [field: SerializeField] public int P1_ID { get; private set; } = 1;
        [field: SerializeField] public int P2_ID { get; private set; } = 2;
    }
}
