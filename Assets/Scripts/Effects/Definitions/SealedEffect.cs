using UnityEngine;

[CreateAssetMenu(fileName = "SealedEffect", menuName = "TypTyp/Effects/SealedEffect")]
public class SealedEffect : StatusEffectDefinition
{
    public override void OnActivate(Player target)
    {
        target.SpellCaster.Sealed = true;
        target.SpellCaster.OnSpellCasted += KillSelf;
    }

    public override void OnDeactivate(Player target)
    {
        target.SpellCaster.Sealed = false;
        target.SpellCaster.OnSpellCasted -= KillSelf;
    }

    public override string GetDefaultValue()
    {
        return "";
    }

    public void KillSelf(CardDefinition _, Player player)
    {
        StatusEffect effectToRemove = null;
        foreach(var effect in player.StatusEffectController.Effects)
        {
            if(effect.Definition is SealedEffect)
            {
                effectToRemove = effect;
                break;
            }       
        }
        if(effectToRemove != null) player.StatusEffectController.RemoveEffect(effectToRemove);
    }
}