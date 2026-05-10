using System.Collections.Generic;
using TypTyp.TextSystem;
using TypTyp.TextSystem.Typable;
using UnityEngine;

public class CardUIManager : MonoBehaviour
{
    [SerializeField] private DeckController deckController;
    [SerializeField] private ManaGainManager manaManager;
    private ITextPipeline textPipeline;
    [SerializeField] private CardUI cardUIPrefab;
    [SerializeField] private Transform cardUIParent;
    [SerializeField] private ScratchAnimation[] scratchs;
    [SerializeField] private CanvasGroup cardsCanvasGroup;
    private Player player;
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
        player = manaManager.GetComponent<Player>();
        CreateSlots(TypTyp.Settings.Instance.HandSize);
    }

    void OnEnable()
    {
        deckController.OnCardDrawnEvent += HandleCardDrawn;
        deckController.OnCardRemovedEvent += HandleCardRemoved;
        deckController.OnDiscountApplied += HandleDiscount;

        manaManager.OnCostModifierChangedEvent += ManaCostModifierChanged;
        PlayerInputEffect.OnSealedChanged += HandleSeal;

        textPipeline.ProcessorAdded += TextPipelineModified;
        textPipeline.ProcessorRemoved += TextPipelineModified;
    }

    void OnDisable()
    {
        deckController.OnCardDrawnEvent -= HandleCardDrawn;
        deckController.OnCardRemovedEvent -= HandleCardRemoved;
        deckController.OnDiscountApplied -= HandleDiscount;

        manaManager.OnCostModifierChangedEvent -= ManaCostModifierChanged;
        PlayerInputEffect.OnSealedChanged -= HandleSeal;

        textPipeline.ProcessorAdded -= TextPipelineModified;
        textPipeline.ProcessorRemoved -= TextPipelineModified;
    }

    private void HandleSeal(Player target, bool applied)
    {
        if (target != player) return;
        foreach (var s in scratchs)
        {
            if (applied) s.Scratch();
            else s.RemoveScratch();
            cardsCanvasGroup.alpha = applied ? 0.5f : 1.0f;
        }
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

    void HandleCardWritten(CardUI cardUI)
    {
        int id = CardRegister.Instance.GetId(cardUI.CardDefinition);
        TypableController tc = cardUI.GetComponentInChildren<TypableController>(true);
        string exactText = tc != null ? tc.Text : cardUI.CardDefinition.Name;
        deckController.RequestPlayCard(id, exactText);
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

    private void HandleCardRemoved(CardEventArgs args)
    {
        var cardId = args.CardId;
        if (cardUIById.TryGetValue(cardId, out CardUI ui))
        {
            cardUIById.Remove(cardId);

            ui.Clear();
            emptySlots.Enqueue(ui);
        }
    }
}
