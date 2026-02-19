using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public SpellData[] spells;

    private ManaSystem manaSystem;
    private CorruptionSystem corruptionSystem;

    private void Awake()
    {
        manaSystem = GetComponent<ManaSystem>();
        corruptionSystem = GetComponent<CorruptionSystem>();
    }

    public bool CanCast(int index)
    {
        if (index < 0 || index >= spells.Length) return false;
        return manaSystem.CurrentMana >= spells[index].ManaCost;
    }

    public void Cast(int index)
    {
        if (!CanCast(index)) return;

        var spell = spells[index];

        manaSystem.ConsumeMana(spell.ManaCost);

        // En MVP aplicamos corrupción a uno mismo
        corruptionSystem.AddCorruption(spell.CorruptionToTarget);
    }
}
