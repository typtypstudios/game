using UnityEngine;

[CreateAssetMenu(fileName = "CorruptionMultEffect", menuName = "TypTyp/Effects/CorruptionMultEffect")]
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
}