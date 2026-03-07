using UnityEngine;

[CreateAssetMenu(fileName = "CardDefinition", menuName = "TypTyp/Cards/CardDefinition")]
public class CardDefinition : ScriptableObject
{
    [field: SerializeField] public SpellDefinition Spell { get; private set; }
    [field: SerializeField] public string CardName { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }
    [field: SerializeField] public Sprite CardImage { get; private set; }
    [field: SerializeField] public int ManaCost { get; private set; } = 1;
}
