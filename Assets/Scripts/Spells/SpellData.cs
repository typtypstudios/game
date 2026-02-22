using UnityEngine;

[CreateAssetMenu(menuName = "Game/Spell")]
public abstract class SpellData : ScriptableObject
{
    [field: SerializeField] public string SpellName { get; private set; }
    [field: SerializeField]public int ManaCost{ get; private set; } = 1;
    [field: SerializeField]public int CorruptionToTarget{ get; private set; } //?
    
    public abstract void ApplyEffect(Player caster, Player[] targets);
}

public class Hechizo : SpellData
{
    public override void ApplyEffect(Player caster, Player[] targets)
    {
        //Apply effect
    }
}