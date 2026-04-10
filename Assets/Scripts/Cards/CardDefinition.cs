using TypTyp.Cults;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDefinition", menuName = "TypTyp/Cards/CardDefinition")]
public class CardDefinition : ADefinition
{
    [field: SerializeField] public CultDefinition Cult { get; private set; }
    [field: SerializeField] public int RequiredLevel { get; private set; } = 0;
    [field: SerializeField] public SpellDefinition Spell { get; private set; }
    [field: SerializeField] public int ManaCost { get; private set; } = 1;

    [Header("Audio")]
    [field: SerializeField] public AudioClip CastSound { get; private set; }
    [field: SerializeField, Range(0f, 1f)] public float CastVolume { get; private set; } = 1f;
}
