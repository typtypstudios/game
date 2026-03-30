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
        if (target.DeckController != null)
        {
            target.DeckController.ShuffleHand();
        }
    }

    public override void OnDeactivate(Player target)
    {
    }
}