using UnityEngine;

[CreateAssetMenu(fileName = "SpellDefinition", menuName = "TypTyp/Spells/SpellDefinition")]
public class SpellDefinition : ScriptableObject
{
    [field: SerializeField] public string SpellName { get; private set; }
    [field: SerializeField] public StatusEffectDefinition[] OnSelfEffects { get; private set; }
    [field: SerializeField] public StatusEffectDefinition[] OnEnemyEffects { get; private set; }
}