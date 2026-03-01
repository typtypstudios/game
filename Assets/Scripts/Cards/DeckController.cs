using System;
using System.Collections.Generic;
using System.Linq;
using TypTyp.TextSystem;
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

    void Awake()
    {
        spellCaster = GetComponentInParent<SpellCaster>();
        UnityEngine.Assertions.Assert.IsNotNull(
            spellCaster,
            "DeckController requires a SpellCaster component in its parents");
    }

    public override void OnNetworkSpawn()
    {
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

    [Rpc(SendTo.Server)]
    void PlayCardRpc(int card, RpcParams rpcParams = default)
    {
        //Voy a debugear todo el estado relevante a la validacion
        Debug.Log($"PlayCardRpc received for card {CardRegister.Instance.GetById(card)} from client {rpcParams.Receive.SenderClientId}");
        //Estado del jugador
        Debug.Log($"Current hand: {string.Join(", ", currentHand.Select(id => CardRegister.Instance.GetById(id)))}");
        Debug.Log($"Current mana: {GetComponent<Player>().CurrentMana.Value}");

        //Validar propietario
        if (rpcParams.Receive.SenderClientId != OwnerClientId)
            return;

        if (!currentHand.Contains(card))
        {
            PlayCardResultRpc(PlayCardRequestResult.NotInHand, card);
            return;
        }

        var cardDef = CardRegister.Instance.GetById(card);
        if (!spellCaster.TryCastSpell(cardDef.Spell))
        {
            PlayCardResultRpc(PlayCardRequestResult.NotEnoughMana, card);
            return;
        }

        ReturnCardToDeck(card);
        int newCard = DrawCard();
        PlayCardResultRpc(PlayCardRequestResult.Success, card);
        ReceiveCardDrawnRpc(newCard);
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
        ReceiveCardDrawnRpc(initialCardIds);
    }

    void UpdateQueueListView()
    {
        Cards = cardQueue.Select(id => CardRegister.Instance.GetById(id)).ToArray();
    }

    //CLIENT SIDE METHODS. Client will receive the cards to play from the server,
    // but the client will handle the actual playing of the cards and the spell casting.
    // This is to ensure responsiveness and a better player experience. The server will
    // validate the actions taken by the client and will have the final say on whether a card play is successful or not.

    public void RequestPlayCard(int cardId)
    {
        if (IsOwner)
            PlayCardRpc(cardId);
    }

    [Rpc(SendTo.Owner)]
    void PlayCardResultRpc(PlayCardRequestResult result, int cardId)
    {
        if (result == PlayCardRequestResult.Success)
        {
            OnCardPlayedEvent?.Invoke(cardId);
            OnCardPlayed.Invoke(CardRegister.Instance.GetById(cardId));
        }
        else
        {
            //Animacion de error al jugar carta
            Debug.LogWarning($"Failed to play card {CardRegister.Instance.GetById(cardId)}: {result}");
        }
    }

    [Rpc(SendTo.Owner)]
    void ReceiveCardDrawnRpc(params int[] cardIds)
    {
        foreach (var card in cardIds)
        {
            var cardDef = CardRegister.Instance.GetById(card);
            OnCardDrawnEvent?.Invoke(card);
            OnCardDrawn.Invoke(cardDef);
        }
    }
}