using System.Collections.Generic;
using System.Linq;
using TypTyp.TextSystem;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    [field: SerializeField] public CardDefinition[] Cards { get; private set; }
    [field: SerializeField] public CardUI[] CardUIs { get; private set; }
    Dictionary<CardDefinition, CardUI> cardUIDict = new();
    private Queue<CardDefinition> cardQueue;
    private SpellCaster spellCaster;

    public void Awake()
    {
        spellCaster = GetComponentInParent<SpellCaster>();
    }

    public void Start()
    {
        Cards.Shuffle(0);
        cardQueue = new(Cards);
        foreach (var cardUI in CardUIs)
        {
            var cardDef = DrawCard();
            cardUI.SetCardInfo(cardDef);
            cardUI.WrittableSpell.OnSpellComplete.AddListener(OnSpellWritten);
        }
    }

    public void PlayCard(CardDefinition card)
    {
        if (spellCaster.TryCastSpellClientSide(card.Spell))
        {
            var cardUI = cardUIDict[card];
            var newCard = DrawCard();
            //Animation de carta jugada y entrada de nueva carta
            cardUI.SetCardInfo(newCard);
            cardUIDict[card] = cardUI;
        }
    }

    public CardDefinition DrawCard()
    {
        var card = cardQueue.Dequeue();
        cardQueue.Enqueue(card);
        UpdateQueue();
        return card;
    }

    void UpdateQueue()
    {
        Cards = cardQueue.ToArray();
    }

    void OnSpellWritten(string spellText)
    {
        CardDefinition cardDef = Cards.FirstOrDefault(c => c.CardName == spellText);
        PlayCard(cardDef);
    }
}