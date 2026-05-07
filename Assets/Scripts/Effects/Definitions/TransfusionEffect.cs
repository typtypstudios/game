using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TransfusionEffect", menuName = "TypTyp/Effects/TransfusionEffect")]
public class TransfusionEffect : StatusEffectDefinition
{
    public override void OnActivate(EffectContext context)
    {
        Player target = context.Target;
        System.Random rng = context.Random;
        if (rng == null)
            return;

        StatusEffectController controller = target.StatusEffectController;
        List<StatusEffect> negativeEffects = new();
        for (int i = 0; i < controller.Effects.Count; i++)
        {
            if (controller.Effects[i].Definition.EffectPolarityType == EffectPolarityType.Bad)
                negativeEffects.Add(controller.Effects[i]);
        }

        if (negativeEffects.Count == 0)
            return;

        int randomEffect = rng.Next(negativeEffects.Count);
        controller.RemoveEffect(negativeEffects[randomEffect]);

        List<StatusEffectDefinition> positiveEffects = StatusEffectRegister.Instance.RegisteredItems
            .Where(e => e.EffectPolarityType == EffectPolarityType.Good)
            .ToList();

        if (positiveEffects.Count == 0)
            return;

        randomEffect = rng.Next(positiveEffects.Count);
        controller.AddEffect(positiveEffects[randomEffect]);
    }

    public override void OnDeactivate(EffectContext context) { }

    public override string GetDefaultValue()
    {
        return "";
    }
}
