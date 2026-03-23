using UnityEngine;

[CreateAssetMenu(fileName = "CorruptionMultEffect", menuName = "TypTyp/Effects/CorruptionMultEffect")]
public class CorruptionMultiplierEffect : StatusEffectDefinition
{
    [SerializeField] private MultiplierType type;
    [SerializeField] private float multiplier = 1;

    public override void OnActivate(Player target)
    {
        //Si hemos gestionado bien que los efectos contrarios se anulen, 
        //no deberíamos tener en cuenta el multiplicador previo, se puede sobrescribir
        if (type == MultiplierType.Hurt) target.CorruptionManager.HurtGainMultiplier = multiplier;
        else if (type == MultiplierType.Heal) target.CorruptionManager.HealGainMultiplier = multiplier;
        else target.CorruptionManager.MistakeGainMultiplier = multiplier;
    }

    public override void OnDeactivate(Player target) 
    {
        if (type == MultiplierType.Hurt) target.CorruptionManager.HurtGainMultiplier = 1;
        else if (type == MultiplierType.Heal) target.CorruptionManager.HealGainMultiplier = 1;
        else target.CorruptionManager.MistakeGainMultiplier = 1;
    }

    public override string GetDefaultValue()
    {
        return $"x{multiplier}";
    }
}

enum MultiplierType
{
    Hurt,
    Heal,
    Mistake
}
