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
    private static int seed;
    public UnityEvent<CardDefinition> OnCardPlayed = new();
    public UnityEvent<CardDefinition> OnCardDrawn = new();
    public event Action<int> OnCardPlayedEvent;
    public event Action<int> OnCardDrawnEvent;
    public event Action<int> OnCardPlayRequestSuccess;
    public event Action<int> OnCardPlayRequestFailed;

    void Awake()
    {
        Cards = Array.Empty<CardDefinition>();
        spellCaster = GetComponentInParent<SpellCaster>();
        UnityEngine.Assertions.Assert.IsNotNull(spellCaster,
            "DeckController requires a SpellCaster component in its parents");
        seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }

    #region Server side

    public void ConfigureServerDeckController(int[] deck)
    {
        if (!IsServer) return;

        deck.Shuffle(seed);
        //just for editor view purposes
        Cards = deck.Select(c => GetCardDefinitionById(c)).ToArray();
        cardQueue = new(deck);
        currentHand = new(Settings.Instance.HandSize);

        DrawInitialCards();
    }

    [Rpc(SendTo.Server)]
    void PlayCardRequestRpc(int card, RpcParams rpcParams = default)
    {
        var validation = ValidatePlayCardRequest(RequestValidationType.Server, card);

        if (validation == PlayCardRequestResult.Success)
        {
            ReturnCardToDeck(card);
            int newCard = DrawCard();
            PlayCard(card);
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

        Debug.Log($"[Deck][Server][PlayCard] cid={OwnerClientId} card={card} spell={SpellRegister.Instance.GetId(spellDef)}");

        spellCaster.CastSpell(cardDef);
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

    private void ReturnCardToDeck(int card)
    {
        currentHand.Remove(card);
        cardQueue.Enqueue(card);

        // Debug.Log($"[Deck][Server][ReturnCard] cid={OwnerClientId} card={card} handSize={currentHand.Count} queue={cardQueue.Count}");
    }

    private void DrawInitialCards()
    {
        var handSize = TypTyp.Settings.Instance.HandSize;

        // Debug.Log($"[Deck][Server][InitialDraw] cid={OwnerClientId} size={handSize}");

        int[] initialCardIds = new int[handSize];

        for (int i = 0; i < handSize && cardQueue.Count > 0; i++)
            initialCardIds[i] = DrawCard();

        // Debug.Log($"[Deck][Server][InitialDrawSend] cid={OwnerClientId} cards={string.Join(",", initialCardIds)}");

        ReceiveCardsDrawnRpc(initialCardIds);
    }

    #endregion

    #region Client Side

    //Client entry point
    public void RequestPlayCard(int card)
    {
        if (!IsOwner) return;

        // Debug.Log($"[Deck][Client][PlayRequest] cid={OwnerClientId} card={card}");

        var validation = ValidatePlayCardRequest(RequestValidationType.Client, card);

        Debug.Log($"[Deck][Client][Validation] cid={OwnerClientId} card={card} res={validation}");

        if (validation == PlayCardRequestResult.Success)
        {
            // Debug.Log($"[Deck][Client][SendRPC] cid={OwnerClientId} card={card}");
            PlayCardRequestRpc(card);
        }
        else
        {
            // Debug.Log($"[Deck][Client][RejectLocal] cid={OwnerClientId} card={card} res={validation}");
        }
    }

    //Server response to play card request
    [Rpc(SendTo.Owner)]
    void PlayCardResultRpc(PlayCardRequestResult result, int playedCard, params int[] receivedCards)
    {
        Debug.Log($"[Deck][Client][PlayResult] cid={OwnerClientId} card={playedCard} res={result} newCards={string.Join(",", receivedCards)}");

        if (result == PlayCardRequestResult.Success)
        {
            OnCardPlayedEvent?.Invoke(playedCard);
            OnCardPlayed.Invoke(CardRegister.Instance.GetById(playedCard));
        }
        else
        {
            // Debug.LogWarning($"[Deck][Client][PlayFailed] card={playedCard} res={result}");
        }

        if (receivedCards.Length > 0)
            ReceiveCardsDrawn(receivedCards);
    }

    [Rpc(SendTo.Owner)]
    void ReceiveCardsDrawnRpc(params int[] cardIds)
    {
        Debug.Log($"[Deck][Client][ReceiveCardsRpc] cid={OwnerClientId} cards={string.Join(",", cardIds)}");
        ReceiveCardsDrawn(cardIds);
    }

    void ReceiveCardsDrawn(params int[] cardIds)
    {
        if (!IsOwner)
        {
            // Debug.LogError("[Deck][Client][ReceiveCards] nonOwner");
            return;
        }

        foreach (var card in cardIds)
        {
            // Debug.Log($"[Deck][Client][CardDrawn] cid={OwnerClientId} card={card}");
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

        var cardDef = GetCardDefinitionById(card);
        var spellCastRequestValidation = spellCaster.ValidateSpellCastRequest(validationType, cardDef);
        if (spellCastRequestValidation != PlayCardRequestResult.Success)
        {
            return spellCastRequestValidation;
        }

        return PlayCardRequestResult.Success;
    }

    #endregion

    #region Helper Methods

    CardDefinition GetCardDefinitionById(int id) => CardRegister.Instance.GetById(id);
    int GetCardId(CardDefinition cardDef) => CardRegister.Instance.GetId(cardDef);

    void UpdateQueueListView()
    {
        Cards = cardQueue.Select(id => CardRegister.Instance.GetById(id)).ToArray();
    }

    #endregion
}