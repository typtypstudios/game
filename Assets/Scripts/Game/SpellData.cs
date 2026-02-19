using UnityEngine;

[CreateAssetMenu(menuName = "Game/Spell")]
public class SpellData : ScriptableObject
{
    public string SpellName;
    public int ManaCost;
    public int CorruptionToTarget;
}
