using UnityEngine;

[CreateAssetMenu(fileName = "GameplayModEffect", menuName = "TypTyp/Effects/GameplayModEffect")]
public class SilenceSpellsEffect : StatusEffectDefinition
{
    public override string GetDefaultValue()
    {
        return "";
    }

    public override void OnActivate(Player target)
    {
        if(target.TryGetComponent<PlayerInputManager>(out var inputManager))
        {
            inputManager.SilenceSpellCasting(true);
        }

        
    }

    public override void OnDeactivate(Player target)
    {
        if (target.TryGetComponent<PlayerInputManager>(out var inputManager))
        {
            inputManager.SilenceSpellCasting(false);
        }
    }
}
