using TypTyp;
using UnityEngine;

[CreateAssetMenu(fileName = "ApplyCorruptionEffect", menuName = "TypTyp/Effects/ApplyCorruptionEffect")]
public class ApplyCorruptionEffect : StatusEffectDefinition
{
    [Range(0, 100)][SerializeField] float corruptionPercentage;
    [SerializeField] bool heal;

    public override void OnActivate(EffectContext context)
    {
        Player target = context.Target;
        if (!target.IsServer) return;
        float corruptionToAdd = corruptionPercentage / 100 * Settings.Instance.MaxCorruption;
        if (heal) corruptionToAdd *= -1;
        target.CorruptionManager.AddCorruption(corruptionToAdd);
    }

    public override void OnDeactivate(EffectContext context) { }

    public override string GetDefaultValue()
    {
        return $"{(int)corruptionPercentage}%";
    }
}

