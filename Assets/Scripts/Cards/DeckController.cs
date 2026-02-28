using System.Collections.Generic;
using System.Linq;
using TypTyp.TextSystem;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Runtime;

public class DeckController : NetworkBehaviour
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

    private void DrawInitialCards()
    {

    }

    void UpdateQueue()
    {
        Cards = cardQueue.ToArray();
    }


    //CLIENT SIDE METHODS. Client will receive the cards to play from the server,
    // but the client will handle the actual playing of the cards and the spell casting.
    // This is to ensure responsiveness and a better player experience. The server will
    // validate the actions taken by the client and will have the final say on whether a card play is successful or not.
    void OnSpellWritten(string spellText)
    {
        CardDefinition cardDef = Cards.FirstOrDefault(c => c.CardName == spellText);
        PlayCard(cardDef);

        RpcTarget.Single(OwnerClientId, RpcTargetUse.Temp);
    }

    [Rpc(SendTo.Owner)]
    void ReceiveCardRpc(int a)
    {
        
    }
}