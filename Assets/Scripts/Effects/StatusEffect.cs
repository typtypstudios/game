using System;
using UnityEngine;

[System.Serializable]
public class StatusEffect : IEquatable<StatusEffect>
{
    [field: SerializeField] public StatusEffectDefinition Definition { get; private set; }
    [field: SerializeField] public Player Target { get; private set; }
    [field: SerializeField] public float RemainingDuration { get; set; }

    public StatusEffect(StatusEffectDefinition definition, Player target)
    {
        Definition = definition;
        Target = target;
    }

    public void Activate()
    {
        if (Target == null)
        {
            Debug.LogWarning("StatusEffect has no target assigned.");
            return;
        }
        RemainingDuration = Definition.DurationValue;
        Definition.OnActivate(Target);
    }

    public void Deactivate()
    {
        RemainingDuration = 0;
        Definition.OnDeactivate(Target);
    }

    public bool Equals(StatusEffect other)
    {
        if (other == null) return false;
        return Definition == other.Definition && Target == other.Target;
    }

    public override bool Equals(object obj) => obj is StatusEffect statusEffect && Equals(statusEffect);
    public override int GetHashCode() => HashCode.Combine(Definition, Target);
}