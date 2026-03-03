using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using TypTyp;

public enum PlayCardRequestResult
{
    Success,
    NotOwner,
    NotEnoughMana,
    SpellOnCooldown,
    NotInHand
}

public enum RequestValidationType
{
    Client,
    Server
}

public class DeckController : NetworkBehaviour
{
    [field: ContextMenuItem("Update Cards View", nameof(UpdateQueueListView))]
    [field: SerializeField] public CardDefinition[] Cards { get; private set; }
    private Queue<int> cardQueue;
    private HashSet<int> currentHand;
    private SpellCaster spellCaster;

    public UnityEvent<CardDefinition> OnCardPlayed = new();
    public UnityEvent<CardDefinition> OnCardDrawn = new();
    public event Action<int> OnCardPlayedEvent;
    public event Action<int> OnCardDrawnEvent;
    public event Action<int> OnCardPlayRequestSuccess;
    public event Action<int> OnCardPlayRequestFailed;

    void Awake()
    {
        spellCaster = GetComponentInParent<SpellCaster>();
        UnityEngine.Assertions.Assert.IsNotNull(
            spellCaster,
            "DeckController requires a SpellCaster component in its parents");
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner) Cards = DeckBuilder.CardsInDeck;
        if (IsServer)
        {
            Cards.Shuffle(0);
            cardQueue = new(Cards.Select(c => CardRegister.Instance.GetId(c)));
            currentHand = new(Settings.Instance.HandSize);
            DrawInitialCards();
        }
        else if (!IsOwner)
        {
            enabled = false;
        }
    }

    #region Server side

    [Rpc(SendTo.Server)]
    void PlayCardRequestRpc(int card, RpcParams rpcParams = default)
    {
        //Voy a debugear todo el estado relevante a la validacion
        Debug.Log($"PlayCardRpc received for card {CardRegister.Instance.GetById(card)} from client {rpcParams.Receive.SenderClientId}");
        //Estado del jugador
        Debug.Log($"Current hand: {string.Join(", ", currentHand.Select(id => CardRegister.Instance.GetById(id)))}");
        Debug.Log($"Current mana: {GetComponent<Player>().CurrentMana.Value}");

        var validation = ValidatePlayCardRequest(RequestValidationType.Server, card);

        if (validation == PlayCardRequestResult.Success)
        {
            ReturnCardToDeck(card);
            int newCard = DrawCard();
            PlayCardResultRpc(validation, card, newCard);
        }
        else
        {
            PlayCardResultRpc(validation, card);
        }
    }

    void PlayCard(int card)
    {
        var cardDef = GetCardDefinitionById(card);
        var spellDef = cardDef.Spell;

        spellCaster.CastSpell(spellDef);
    }

    private int DrawCard()
    {
        if (cardQueue.Count == 0)
        {
            Debug.LogError("Deck empty");
            return -1;
        }
        var card = cardQueue.Dequeue();
        currentHand.Add(card);
        //UpdateQueueListView();
        return card;
    }

    #endregion

    private void ReturnCardToDeck(int card)
    {
        currentHand.Remove(card);
        cardQueue.Enqueue(card);
        //UpdateQueueListView();
    }

    private void DrawInitialCards()
    {
        var handSize = TypTyp.Settings.Instance.HandSize;
        int[] initialCardIds = new int[handSize];
        for (int i = 0; i < TypTyp.Settings.Instance.HandSize && cardQueue.Count > 0; i++)
        {
            initialCardIds[i] = DrawCard();
        }
        ReceiveCardsDrawnRpc(initialCardIds);
    }

    #region Client Side

    //Client entry point
    public void RequestPlayCard(int card)
    {
        if (!IsOwner) return;

        //First, validate on client
        var validation = ValidatePlayCardRequest(RequestValidationType.Client, card);
        Debug.LogFormat("Play card request.\n Card: {0}\n Client validation = {1}", card, validation);

        if (validation == PlayCardRequestResult.Success)
        {
            //Activate local pre-animations
            //Send request to server 
            PlayCardRequestRpc(card);
        }
        else
        {
            //Handle cast fail
        }
    }

    //Server response to play card request
    [Rpc(SendTo.Owner)]
    void PlayCardResultRpc(PlayCardRequestResult result, int playedCard, params int[] receivedCards)
    {
        if (result == PlayCardRequestResult.Success)
        {
            OnCardPlayedEvent?.Invoke(playedCard);
            OnCardPlayed.Invoke(CardRegister.Instance.GetById(playedCard));
        }
        else
        {
            //Client said card can be played, but server said it cannot -> Conciliate
            Debug.LogWarning($"Failed to play card {CardRegister.Instance.GetById(playedCard)}: {result}");
        }

        if (receivedCards.Length > 0)
            ReceiveCardsDrawn(receivedCards);
    }

    [Rpc(SendTo.Owner)]
    void ReceiveCardsDrawnRpc(params int[] cardIds)
    {
        ReceiveCardsDrawn(cardIds);
    }

    void ReceiveCardsDrawn(params int[] cardIds)
    {
        if (!IsOwner)
        {
            Debug.LogError("Cards received by other than owner.", this);
        }
        foreach (var card in cardIds)
        {
            var cardDef = CardRegister.Instance.GetById(card);
            OnCardDrawnEvent?.Invoke(card);
            OnCardDrawn.Invoke(cardDef);
        }
    }

    #endregion

    #region Validation

    //De momento la validacion sigue el mismo flujo en server y en cliente
    PlayCardRequestResult ValidatePlayCardRequest(RequestValidationType validationType, int card)
    {
        if (validationType == RequestValidationType.Server)
        {
            if (!currentHand.Contains(card))
            {
                return PlayCardRequestResult.NotInHand;
            }
        }

        var spellDef = GetCardDefinitionById(card).Spell;
        var spellCastRequestValidation = spellCaster.ValidateSpellCastRequest(validationType, spellDef);
        if (spellCastRequestValidation != PlayCardRequestResult.Success)
        {
            return spellCastRequestValidation;
        }

        return PlayCardRequestResult.Success;
    }

    #endregion

    #region Helper Methods

    CardDefinition GetCardDefinitionById(int id) => CardRegister.Instance.GetById(id);

    void UpdateQueueListView()
    {
        Cards = cardQueue.Select(id => CardRegister.Instance.GetById(id)).ToArray();
    }

    #endregion
}