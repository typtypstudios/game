using TypTyp;
using UnityEngine;

[CreateAssetMenu(fileName = "InstabilityEffect", menuName = "TypTyp/Effects/InstabilityEffect")]
public class InstabilityEffect : StatusEffectDefinition
{
    [Range(0, 100)][SerializeField] float corruptionPerBar; //Porcentaje

    public override void OnActivate(Player target)
    {
        if (!target.IsServer) return;
        target.SpellCaster.OnSpellCasted += ManageCastedSpell;
    }

    public override void OnDeactivate(Player target) 
    {
        if (!target.IsServer) return;
        target.SpellCaster.OnSpellCasted -= ManageCastedSpell;
    }

    public override string GetDefaultValue()
    {
        return $"{corruptionPerBar}%";
    }

    private void ManageCastedSpell(CardDefinition card, Player player)
    {
        int cost = card.ManaCost + player.ManaManager.CostModifier;
        float damage = cost * corruptionPerBar / 100 * Settings.Instance.MaxCorruption;
        player.CorruptionManager.AddCorruption(damage);
    }
}
