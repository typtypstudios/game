using UnityEngine;

public enum EffectPolarityType
{
    Good,
    Bad
}

public enum EffectDurationType
{
    Immediate,
    Time,
    Lines,
    Permanent
}

public abstract class StatusEffectDefinition : ADefinition
{
    [field: SerializeField] public EffectDurationType DurationType { get; private set; }
    [field: SerializeField] public StatusEffectCategory Category { get; private set; }
    [field: SerializeField] public EffectPolarityType EffectPolarityType { get; private set; } = EffectPolarityType.Bad;
    [field: SerializeField] public float DurationValue { get; private set; } // Tiempo en segundos o numero de líneas, dependiendo del tipo de duracion

    public abstract void OnActivate(Player target);
    public abstract void OnDeactivate(Player target);
    public abstract string GetDefaultValue();

    public bool IsOpposite(StatusEffectDefinition other)
    {
        var haveCategory = Category != null && other.Category != null;
        return haveCategory && Category == other.Category && EffectPolarityType != other.EffectPolarityType;
    }
}