using System;
using UnityEngine;

[Serializable]
public class StatusEffect : IEquatable<StatusEffect>
{
    [field: SerializeField] public StatusEffectDefinition Definition { get; private set; }
    [field: SerializeField] public Player Target { get; private set; }
    [field: SerializeField] public float RemainingDuration { get; set; }
    [field: SerializeField] public int FirstAffectedLineIndex { get; private set; } = -1;
    [field: SerializeField] public int LastAffectedLineIndex { get; private set; } = -1;

    public StatusEffect(StatusEffectDefinition definition, Player target)
    {
        Definition = definition;
        Target = target;
    }

    public void Activate(EffectContext context)
    {
        if (Target == null)
        {
            Debug.LogWarning("StatusEffect has no target assigned.");
            return;
        }
        RemainingDuration = Definition.DurationValue;
        Definition.OnActivate(context);
    }

    public void SetAffectedLineWindow(int firstLineIndex, int lineCount)
    {
        FirstAffectedLineIndex = firstLineIndex;
        LastAffectedLineIndex = firstLineIndex + Mathf.Max(0, lineCount) - 1;
    }

    public bool AffectsLine(int lineIndex)
    {
        return FirstAffectedLineIndex >= 0 &&
            lineIndex >= FirstAffectedLineIndex &&
            lineIndex <= LastAffectedLineIndex;
    }

    public void Deactivate(EffectContext context)
    {
        RemainingDuration = 0;
        Definition.OnDeactivate(context);
    }

    public bool Equals(StatusEffect other)
    {
        if (other == null) return false;
        return Definition == other.Definition && Target == other.Target;
    }

    public override bool Equals(object obj) => obj is StatusEffect statusEffect && Equals(statusEffect);
    public override int GetHashCode() => HashCode.Combine(Definition, Target);
}
