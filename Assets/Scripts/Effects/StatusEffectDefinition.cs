using UnityEngine;

public enum EffectTargetType
{
    Self,
    Enemy,
    All
}

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

[CreateAssetMenu(fileName = "StatusEffect", menuName = "TypTyp/Effects/StatusEffectDefinition")]
public abstract class StatusEffectDefinition : ScriptableObject
{
    [field: SerializeField] public string EffectName { get; private set; }
    [field: SerializeField] public EffectDurationType DurationType { get; private set; }
    [field: SerializeField] public EffectPolarityType EffectPolarityType { get; private set; } = EffectPolarityType.Bad;
    [field: SerializeField] public EffectTargetType TargetType { get; private set; }
    [field: SerializeField] public float DurationValue { get; private set; } // Tiempo en segundos o número de líneas, dependiendo del tipo de duración
    [field: SerializeField, TextArea] public string Description { get; private set; }

    public abstract void OnActivate(Player target);
    public abstract void OnDeactivate(Player target);
}