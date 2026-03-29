using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TransfusionEffect", menuName = "TypTyp/Effects/TransfusionEffect")]
public class TransfusionEffect : StatusEffectDefinition
{
    //Generador compartido de números en base a los nombres de los jugadores,
    //ya que el controller no está en network
    private static System.Random rand = null; 

    public override void OnActivate(Player target)
    {
        if (rand == null) rand = new(Utils.GetSeedFromNames());
        StatusEffectController controller = target.StatusEffectController;
        List<StatusEffect> negativeEffects = new();
        for(int i = 0; i < controller.Effects.Count; i++) 
        {
            if (controller.Effects[i].Definition.EffectPolarityType == EffectPolarityType.Bad)
                negativeEffects.Add(controller.Effects[i]);
        }
        if (negativeEffects.Count == 0) return;
        int randomEffect = rand.Next(negativeEffects.Count);
        controller.RemoveEffect(negativeEffects[randomEffect]);
        List<StatusEffectDefinition> positiveEffects = StatusEffectRegister.Instance.RegisteredItems.
            Where((e) => e.EffectPolarityType == EffectPolarityType.Good).ToList();
        randomEffect = rand.Next(positiveEffects.Count);
        controller.AddEffect(positiveEffects[randomEffect]);
    }

    public override void OnDeactivate(Player target) { }

    public override string GetDefaultValue()
    {
        return $"";
    }
}
