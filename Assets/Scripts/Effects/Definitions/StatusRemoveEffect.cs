using UnityEngine;

[CreateAssetMenu(fileName = "StatusRemoveEffect", menuName = "TypTyp/Effects/StatusRemoveEffect")]
public class StatusRemoveEffect : StatusEffectDefinition
{
    [Min(1)][SerializeField] private int numEffectsToRemove = 1;
    [SerializeField] private EffectPolarityType polarityToRemove = EffectPolarityType.Bad;

    public override void OnActivate(EffectContext context)
    {
        Player target = context.Target;
        StatusEffectController controller = target.StatusEffectController;
        int numEffects = controller.Effects.Count;
        for(int i = numEffects - 1; i >= 0; i--) //Por defecto se borran antes los últimos
        {
            if (controller.Effects[i].Definition.EffectPolarityType == polarityToRemove)
                controller.RemoveEffect(controller.Effects[i]);
            if (--numEffectsToRemove == 0) return;
        }
    }

    public override void OnDeactivate(EffectContext context) { }

    public override string GetDefaultValue()
    {
        return $"{numEffectsToRemove}";
    }
}

