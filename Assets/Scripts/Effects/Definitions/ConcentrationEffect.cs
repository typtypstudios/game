using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ConcentrationEffect", menuName = "TypTyp/Effects/ConcentrationEffect")]
public class ConcentrationEffect : StatusEffectDefinition
{
    [SerializeField] private int inkDiscount = 1;

    public override void OnActivate(EffectContext context)
    {
        Player target = context.Target;
        System.Random rng = context.Random;
        if (rng == null)
            return;

        CardDefinition[] cards = target.DeckController.Cards.OrderBy((_) => rng.Next()).ToArray();
        foreach(var card in cards)
        {
            if (target.DeckController.TryApplyDiscount(card, inkDiscount)) return;
        }
    }

    public override void OnDeactivate(EffectContext context) { }

    public override string GetDefaultValue()
    {
        return $"{inkDiscount}";
    }
}

