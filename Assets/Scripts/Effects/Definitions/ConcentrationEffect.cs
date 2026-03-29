using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ConcentrationEffect", menuName = "TypTyp/Effects/ConcentrationEffect")]
public class ConcentrationEffect : StatusEffectDefinition
{
    [SerializeField] private int inkDiscount = 1;
    private static System.Random rand = null;

    public override void OnActivate(Player target)
    {
        rand ??= new(Utils.GetSeedFromNames());
        CardDefinition[] cards = target.DeckController.Cards.OrderBy((_) => rand.Next()).ToArray();
        foreach(var card in cards)
        {
            if (target.DeckController.TryApplyDiscount(card, inkDiscount)) return;
        }
    }

    public override void OnDeactivate(Player target) { }

    public override string GetDefaultValue()
    {
        return $"{inkDiscount}";
    }
}
