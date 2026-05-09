using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInputEffect", menuName = "TypTyp/Effects/PlayerInputEffect")]
public class PlayerInputEffect : StatusEffectDefinition
{
    public static event Action<Player, bool> OnSealedChanged;

    public override string GetDefaultValue()
    {
        return "";
    }

    public override void OnActivate(EffectContext context)
    {
        Player target = context.Target;
        if(target.TryGetComponent<PlayerInputManager>(out var inputManager))
        {
            switch (DurationType)
            {
                case EffectDurationType.Immediate:
                    inputManager.SwapEffect();
                    break;

                case EffectDurationType.Time:
                    OnSealedChanged?.Invoke(target, true);
                    inputManager.SilenceSpellEffect(true);
                    break;
            }
        }        
    }

    public override void OnDeactivate(EffectContext context)
    {
        Player target = context.Target;
        if (target.TryGetComponent<PlayerInputManager>(out var inputManager))
        {
            switch (DurationType)
            {
                case EffectDurationType.Immediate:                    
                    break;

                case EffectDurationType.Time:
                    OnSealedChanged?.Invoke(target, false);
                    inputManager.SilenceSpellEffect(false);
                    break;
            }
        }
    }
}

