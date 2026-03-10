using System;
using System.Collections.Generic;
using UnityEngine;

public class CardUIManager : MonoBehaviour
{
    [SerializeField] private DeckController deckController;
    [SerializeField] private ManaGainManager manaManager;
    [SerializeField] private CardUI cardUIPrefab;
    [SerializeField] private Transform cardUIParent;

    private Dictionary<int, CardUI> cardUIById = new();
    private Queue<CardUI> emptySlots = new();

    void Awake()
    {
        if (!deckController)
            deckController = GetComponentInParent<DeckController>();

        UnityEngine.Assertions.Assert.IsNotNull(
            deckController,
            $"CardUIManager requires a reference to {nameof(DeckController)}");

        if (!manaManager)
            manaManager = GetComponentInParent<ManaGainManager>();

        UnityEngine.Assertions.Assert.IsNotNull(
            manaManager,
            $"CardUIManager requires a reference to {nameof(ManaGainManager)}"
        );
    }

    void Start()
    {
        CreateSlots(TypTyp.Settings.Instance.HandSize);
    }

    void OnEnable()
    {
        deckController.OnCardDrawnEvent += HandleCardDrawn;
        deckController.OnCardPlayedEvent += HandleCardPlayed;

        manaManager.OnCostModifierChangedEvent += ManaCostModifierChanged;
    }

    void OnDisable()
    {
        deckController.OnCardDrawnEvent -= HandleCardDrawn;
        deckController.OnCardPlayedEvent -= HandleCardPlayed;
    }

    void CreateSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var ui = Instantiate(cardUIPrefab, cardUIParent);
            ui.OnCardWritten.AddListener(HandleCardWritten);

            emptySlots.Enqueue(ui);
        }
    }

    void HandleCardDrawn(CardEventArgs args)
    {
        var cardId = args.CardId;
        if (emptySlots.Count == 0)
        {
            Debug.LogWarning("No empty slot available for drawn card.");
            return;
        }

        var slot = emptySlots.Dequeue();
        var def = CardRegister.Instance.GetById(cardId);

        slot.BindCardDefinition(def, manaManager.CostModifier);
        cardUIById[cardId] = slot;
    }

    void HandleCardPlayed(CardEventArgs args)
    {
        var cardId = args.CardId;
        if (!cardUIById.TryGetValue(cardId, out var ui))
        {
            Debug.LogWarning($"No CardUI found for cardId {cardId}", this);
            return;
        }

        cardUIById.Remove(cardId);

        ui.Clear(); // método que debes añadir a CardUI
        emptySlots.Enqueue(ui);
    }

    void HandleCardWritten(CardUI cardUI)
    {
        int id = CardRegister.Instance.GetId(cardUI.CardDefinition);
        deckController.RequestPlayCard(id);
    }

    private void ManaCostModifierChanged(int costModifier)
    {
        foreach (var card in cardUIById.Values)
        {
            card.UpdateManaCostModifier(costModifier);
        }
    }
}