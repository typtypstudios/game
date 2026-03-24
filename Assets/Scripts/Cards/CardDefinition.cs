using UnityEngine;

[CreateAssetMenu(fileName = "CardDefinition", menuName = "TypTyp/Cards/CardDefinition")]
public class CardDefinition : ADefinition
{
    [field: SerializeField] public SpellDefinition Spell { get; private set; }
    [field: SerializeField] public int ManaCost { get; private set; } = 1;
}
