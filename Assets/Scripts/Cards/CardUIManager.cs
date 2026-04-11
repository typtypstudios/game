using System.Collections.Generic;
using System.Linq;
using TypTyp.TextSystem;
using UnityEngine;

public class CardUIManager : MonoBehaviour
{
    [SerializeField] private DeckController deckController;
    [SerializeField] private ManaGainManager manaManager;
    private ITextPipeline textPipeline;
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

        if (textPipeline == null)
            textPipeline = GetComponentInParent<ITextPipeline>();

        UnityEngine.Assertions.Assert.IsNotNull(
            textPipeline,
            $"CardUIManager requires a reference to {nameof(ITextPipeline)}"
        );
        CreateSlots(TypTyp.Settings.Instance.HandSize);
    }

    void OnEnable()
    {
        deckController.OnCardDrawnEvent += HandleCardDrawn;
        deckController.OnCardPlayedEvent += HandleCardPlayed;
        deckController.OnDiscountApplied += HandleDiscount;
        deckController.OnShuffledEvent += HandleHandShuffled;

        manaManager.OnCostModifierChangedEvent += ManaCostModifierChanged;

        textPipeline.ProcessorAdded += TextPipelineModified;
        textPipeline.ProcessorRemoved += TextPipelineModified;
    }

    void OnDisable()
    {
        deckController.OnCardDrawnEvent -= HandleCardDrawn;
        deckController.OnCardPlayedEvent -= HandleCardPlayed;
        deckController.OnDiscountApplied -= HandleDiscount;
        deckController.OnShuffledEvent -= HandleHandShuffled;

        manaManager.OnCostModifierChangedEvent -= ManaCostModifierChanged;

        textPipeline.ProcessorAdded -= TextPipelineModified;
        textPipeline.ProcessorRemoved -= TextPipelineModified;
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

        slot.BindCardDefinition(def, textPipeline, manaManager.CostModifier);
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

    private void TextPipelineModified(ITextProcessor processor)
    {
        foreach (var card in cardUIById.Values)
        {
            card.UpdateCardName(textPipeline);
        }
    }

    private void HandleDiscount(CardDefinition _)
    {
        foreach (var card in cardUIById.Values)
        {
            card.UpdateManaCostModifier(manaManager.CostModifier);
        }
    }

    private void HandleHandShuffled(int[] cardsToClear)
    {
        foreach (int cardId in cardsToClear)
        {
            if (cardUIById.TryGetValue(cardId, out CardUI ui))
            {
                cardUIById.Remove(cardId);

                ui.Clear();
                emptySlots.Enqueue(ui);
            }
        }
    }
    public IEnumerable<string> GetHandSpellNames()
    {
        return cardUIById.Values.Select(ui => ui.CardDefinition.name);
    }

}