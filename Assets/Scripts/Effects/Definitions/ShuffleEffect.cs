using UnityEngine;

[CreateAssetMenu(fileName = "ShuffleEffect", menuName = "TypTyp/Effects/ShuffleEffect")]
public class ShuffleEffect : StatusEffectDefinition
{
    public override string GetDefaultValue()
    {
        return "";
    }

    public override void OnActivate(Player target)
    {
    }

    public override void OnDeactivate(Player target)
    {
    }
}