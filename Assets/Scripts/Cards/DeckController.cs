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

public readonly struct CardEventArgs
{
    public readonly ulong PlayerId;
    public readonly int CardId;

    public CardEventArgs(ulong playerId, int cardId)
    {
        CardId = cardId;
        PlayerId = playerId;
    }
}

[RequireComponent(typeof(Player))]
public class DeckController : NetworkBehaviour
{
    [field: ContextMenuItem("Update Cards View", nameof(UpdateQueueListView))]
    [field: SerializeField] public CardDefinition[] Cards { get; private set; }
    private readonly Dictionary<CardDefinition, int> discounts = new();

    private Player player;
    private Queue<int> cardQueue;
    private HashSet<int> currentHand;
    private SpellCaster spellCaster;
    private static int seed;

    //Events
    public UnityEvent<CardDefinition> OnCardPlayed = new();
    public UnityEvent<CardDefinition> OnCardDrawn = new();
    public event Action<CardEventArgs> OnCardPlayedEvent;
    public event Action<CardEventArgs> OnCardDrawnEvent;

    public event Action<CardEventArgs> OnCardPlayRequestSuccess;
    public event Action<CardEventArgs> OnCardPlayRequestFailed;
    public event Action<CardDefinition> OnDiscountApplied;
    public event Action<int[]> OnShuffledEvent;
    //Static events
    public static event Action<CardEventArgs> OnAnyCardPlayedEvent;
    public static event Action<CardEventArgs> OnAnyCardDrawEvent;


    void Awake()
    {
        player = GetComponent<Player>();
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
        SetCardsClientRpc(deck);
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
            PlayCardServer(card);
            PlayCardResultRpc(validation, card, newCard);//Owner
            PlayCardRpc(card);//Not Owner
        }
        else
        {
            PlayCardResultRpc(validation, card);
        }
    }

    void PlayCardServer(int card)
    {
        var cardDef = GetCardDefinitionById(card);

        Debug.Log($"[Deck][Server][PlayCard] cid={OwnerClientId} card={card} spell={SpellRegister.Instance.GetId(cardDef.Spell)}");

        if (player.ManaManager.ConsumeMana(cardDef.ManaCost - GetDiscount(cardDef)))
        {
            // spellCaster.CastSpell(cardDef.Spell);
        }
        else
        {
            Debug.LogError($"[Deck][Server][PlayCard] cid={OwnerClientId} card={card} NotEnoughMana");
        }
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

    public void ShuffleHand()
    {
        if (!IsServer) return;

        int[] currentCards = currentHand.ToArray();
        foreach (int card in currentCards)
        {
            ReturnCardToDeck(card);
        }

        // Barajar
        List<int> shuffledList = cardQueue.OrderBy(x => UnityEngine.Random.value).ToList();
        cardQueue = new Queue<int>(shuffledList);


        int[] newHand = new int[TypTyp.Settings.Instance.HandSize];
        for (int i = 0; i < newHand.Length; i++)
        {
            newHand[i] = DrawCard();
        }

        ShuffleHandRpc(newHand);
    }

    #endregion

    #region Client Side

    [Rpc(SendTo.ClientsAndHost)]
    private void SetCardsClientRpc(int[] cards)
    {
        Cards = cards.Select(c => GetCardDefinitionById(c)).ToArray();
    }

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
    [Rpc(SendTo.Owner, InvokePermission = RpcInvokePermission.Server)]
    void PlayCardResultRpc(PlayCardRequestResult result, int playedCard, params int[] receivedCards)
    {
        Debug.Log($"[Deck][Client][PlayResult] cid={OwnerClientId} card={playedCard} res={result} newCards={string.Join(",", receivedCards)}");

        if (result == PlayCardRequestResult.Success)
        {
            // OnCardPlayedEvent?.Invoke(playedCard);
            // OnCardPlayed.Invoke(CardRegister.Instance.GetById(playedCard));
            PlayCardClient(playedCard);
        }
        else
        {
            // Debug.LogWarning($"[Deck][Client][PlayFailed] card={playedCard} res={result}");
        }

        if (receivedCards.Length > 0)
            ReceiveCardsDrawn(receivedCards);
    }

    [Rpc(SendTo.NotOwner, InvokePermission = RpcInvokePermission.Server)]
    void PlayCardRpc(int playedCard)
    {
        PlayCardClient(playedCard);
    }

    void PlayCardClient(int playedCard)//And server/host
    {
        var cardDef = GetCardDefinitionById(playedCard);
        player.SpellCaster.CastSpell(cardDef);

        //Events
        CardEventArgs eventArgs = new(OwnerClientId, playedCard);
        OnCardPlayedEvent?.Invoke(eventArgs);
        OnCardPlayed?.Invoke(cardDef);
        OnAnyCardPlayedEvent?.Invoke(eventArgs);
    }

    [Rpc(SendTo.Owner, InvokePermission = RpcInvokePermission.Server)]

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
            CardEventArgs eventArgs = new(OwnerClientId, card);
            OnCardDrawnEvent?.Invoke(eventArgs);
            OnCardDrawn?.Invoke(cardDef);
            OnAnyCardDrawEvent?.Invoke(eventArgs);
        }
    }

    [Rpc(SendTo.Owner, InvokePermission = RpcInvokePermission.Server)]
    private void ShuffleHandRpc(params int[] newCards)
    {
        Debug.Log($"[Deck][Client][ShuffleHandRpc] cid={OwnerClientId} newCards={string.Join(",", newCards)}");
        OnShuffledEvent?.Invoke(newCards);
        ReceiveCardsDrawn(newCards);
    }

    public bool TryApplyDiscount(CardDefinition card, int discount)
    {
        if (card.ManaCost - GetDiscount(card) - discount < 0) return false;
        discounts[card] += discount;
        OnDiscountApplied?.Invoke(card);
        return true;
    }

    public int GetDiscount(CardDefinition card)
    {
        if (!discounts.ContainsKey(card)) discounts[card] = 0;
        return discounts[card];
    }

    #endregion

    #region Validation

    //De momento la validacion sigue el mismo flujo en server y en cliente
    PlayCardRequestResult ValidatePlayCardRequest(RequestValidationType validationType, int card)
    {
        if (validationType == RequestValidationType.Server)
        {
            //Hand validation
            if (!currentHand.Contains(card))
            {
                return PlayCardRequestResult.NotInHand;
            }
        }

        var cardDef = GetCardDefinitionById(card);
        if (!player.ManaManager.CanAfford(cardDef.ManaCost - GetDiscount(cardDef)))
        {
            return PlayCardRequestResult.NotEnoughMana;
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