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

public abstract class StatusEffectDefinition : ScriptableObject
{
    [field: SerializeField] public string EffectName { get; private set; }
    [field: SerializeField] public EffectDurationType DurationType { get; private set; }
    [field: SerializeField] public StatusEffectCategory Category { get; private set; }
    [field: SerializeField] public EffectPolarityType EffectPolarityType { get; private set; } = EffectPolarityType.Bad;
    [field: SerializeField] public float DurationValue { get; private set; } // Tiempo en segundos o numero de líneas, dependiendo del tipo de duracion
    [field: SerializeField, TextArea] public string Description { get; private set; }
    [field: SerializeField] public Sprite ImageUI { get; private set; }

    public abstract void OnActivate(Player target);
    public abstract void OnDeactivate(Player target);

    public bool IsOpposite(StatusEffectDefinition other)
    {
        var haveCategory = Category != null && other.Category != null;
        return haveCategory && Category == other.Category && EffectPolarityType != other.EffectPolarityType;
    }
}