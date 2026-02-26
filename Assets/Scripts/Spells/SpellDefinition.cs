using UnityEngine;

[CreateAssetMenu(fileName = "SpellDefinition", menuName = "TypTyp/Spells/SpellDefinition")]
public abstract class SpellDefinition : ScriptableObject
{
    [field: SerializeField] public string SpellName { get; private set; }
    [field: SerializeField] public int ManaCost { get; private set; } = 1;
    [field: SerializeField] public StatusEffectDefinition[] StatusEffects { get; private set; } 

    // public abstract void ApplySpell(Player caster, Player[] targets);
}