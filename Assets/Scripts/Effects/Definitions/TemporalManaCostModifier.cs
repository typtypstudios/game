using UnityEngine;

[CreateAssetMenu(fileName = "TemporalManaCostModifier", menuName = "TypTyp/Effects/TemporalManaCostModifier")]
public class TemporalManaCostModifier : StatusEffectDefinition
{
    [field: SerializeField] public int CostModifier { get; private set; } = 1;

    public override void OnActivate(Player target)
    {
        // if (!target.IsServer) return;

        target.ManaManager.AddCostModifier(CostModifier);
    }

    public override void OnDeactivate(Player target)
    {
        // if (!target.IsServer) return;

        target.ManaManager.AddCostModifier(-CostModifier);
    }

    public override string GetDefaultValue()
    {
        return $"{(CostModifier >= 0 ? "+" : "-")}{CostModifier}";
    }
}