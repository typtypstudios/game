using TMPro;
using UnityEngine;
using UnityEngine.Events;
using TypTyp.TextSystem;
using TypTyp.TextSystem.Typable;
using System.Collections.Generic;
using TypTyp;
using System;

public class CardUI : MonoBehaviour
{
    [field: SerializeField] public CardDefinition CardDefinition { get; private set; }

    [Header("Text UI")]
    [SerializeField] private TMP_Text cardName;

    [Header("Legacy Orbs UI")]
    [SerializeField] private Transform cardCostLayout;
    [SerializeField] private GameObject inkOrbPrefab;

    [Header("Runtime Visuals")]
    [SerializeField] private CardVisualPresenter visualPresenter;

    private TypableController typableController;
    private DeckController deckController;
    private bool useVisualPresenter;
    public UnityEvent<CardUI> OnCardWritten = new();
    private readonly List<InkOrb> orbs = new();
    private int manaCost;
    private Player player;
    public event Action<CardUI, float, bool> OnIdxChanged;

    void Awake()
    {
        typableController = GetComponentInChildren<TypableController>();
        player = GetComponentInParent<Player>();
        deckController = player.DeckController;
        UnityEngine.Assertions.Assert.IsNotNull(typableController);

        useVisualPresenter = visualPresenter != null;

        player.CurrentMana.OnValueChanged += HandleManaChange;
        for (int i = 0; i <= Settings.Instance.NumManaBars; i++)
        {
            orbs.Add(Instantiate(inkOrbPrefab, cardCostLayout).GetComponent<InkOrb>());
            orbs[i].CompletelyFill(false);
        }
    }

    void OnEnable()
    {
        typableController.OnComplete += OnSpellWritten;
        typableController.OnChanged += OnChanged;
    }

    void OnDisable()
    {
        typableController.OnComplete -= OnSpellWritten;
        typableController.OnChanged -= OnChanged;
    }

    public void BindCardDefinition(CardDefinition def, ITextPipeline pipeline, int costModifier = 0)
    {
        CardDefinition = def;
        UpdateCardName(pipeline);
        UpdateManaCostModifier(costModifier);
        if (useVisualPresenter)
        {
            visualPresenter.SetCard(def, manaCost, Mathf.FloorToInt(player.CurrentMana.Value));
        }
    }

    public void UpdateCardName(ITextPipeline pipeline)
    {
        var text = pipeline.ProcessText(CardDefinition.Name);
        cardName.text = text;
        typableController.SetText(text);
    }

    public void UpdateManaCostModifier(int costModifier)
    {
        manaCost = Mathf.Max(0, CardDefinition.ManaCost - deckController.GetDiscount(CardDefinition) + costModifier);

        for (int i = 0; i < orbs.Count; i++)
        {
            bool isActive = i + 1 <= manaCost;
            orbs[i].gameObject.SetActive(isActive);
            if (!isActive)
            {
                continue;
            }

            bool isFilled = player.CurrentMana.Value >= i + 1;
            orbs[i].CompletelyFill(isFilled);
        }

        if (useVisualPresenter)
        {
            visualPresenter.SetResolvedCost(manaCost);
        }
    }

    private void HandleManaChange(float prevMana, float newMana)
    {
        for (int i = 0; i < orbs.Count; i++)
        {
            bool isFilled = newMana >= i + 1;
            orbs[i].CompletelyFill(isFilled);
        }
        
        if (useVisualPresenter)
        {
            visualPresenter.SetMana(Mathf.FloorToInt(newMana));
        }
    }

    public void Clear()
    {
        CardDefinition = null;

        foreach (var orb in orbs)
        {
            orb.gameObject.SetActive(false);
        }

        if (useVisualPresenter)
        {
            visualPresenter.Clear();
        }

        cardName.text = string.Empty;
    }

    void OnSpellWritten()
    {
        OnCardWritten?.Invoke(this);
    }

    private void OnChanged()
    {
        float progress = (float)typableController.Idx / typableController.Text.Length;
        OnIdxChanged?.Invoke(this, progress, player.CurrentMana.Value >= manaCost);
    }

    // El modo de input se gestiona via TypingInputListener en el propio objeto.
}
