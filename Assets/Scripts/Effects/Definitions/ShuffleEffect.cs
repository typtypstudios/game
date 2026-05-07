using UnityEngine;

[CreateAssetMenu(fileName = "ShuffleEffect", menuName = "TypTyp/Effects/ShuffleEffect")]
public class ShuffleEffect : StatusEffectDefinition
{
    public override string GetDefaultValue()
    {
        return "";
    }

    public override void OnActivate(EffectContext context)
    {
        Player target = context.Target;
    }

    public override void OnDeactivate(EffectContext context)
    {
        Player target = context.Target;
    }
}
