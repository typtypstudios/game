using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TypTyp.TextSystem;
using TypTyp.TextSystem.Typable;
using System.Collections.Generic;
using TypTyp;
using System;

public class CardUI : MonoBehaviour
{
    [field: SerializeField] public CardDefinition CardDefinition { get; private set; }

    [Header("Legacy UI")]
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private Transform cardCostLayout;
    [SerializeField] private GameObject inkOrbPrefab;
    [SerializeField] private Color blockedColor = Color.grey;

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
        if (!useVisualPresenter)
        {
            for (int i = 0; i <= Settings.Instance.NumManaBars; i++)
            {
                orbs.Add(Instantiate(inkOrbPrefab, cardCostLayout).GetComponent<InkOrb>());
                orbs[i].CompletelyFill(false);
            }
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

        if (!useVisualPresenter && cardImage)
        {
            cardImage.sprite = def.Image;
        }

        UpdateCardName(pipeline);
        UpdateManaCostModifier(costModifier);
        if (useVisualPresenter)
        {
            visualPresenter.SetCard(def, manaCost, Mathf.FloorToInt(player.CurrentMana.Value));
        }
        else
        {
            UpdateImageColor(player.CurrentMana.Value);
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

        if (useVisualPresenter)
        {
            visualPresenter.SetResolvedCost(manaCost);
            return;
        }

        for (int i = 0; i < orbs.Count; i++)
        {
            orbs[i].gameObject.SetActive(i + 1 <= manaCost);
        }

        UpdateImageColor(player.CurrentMana.Value);
    }

    private void HandleManaChange(float prevMana, float newMana)
    {
        if (useVisualPresenter)
        {
            visualPresenter.SetMana(Mathf.FloorToInt(newMana));
            return;
        }

        for (int i = 0; i < orbs.Count; i++)
        {
            bool isFilled = newMana >= i + 1;
            orbs[i].CompletelyFill(isFilled);
        }

        UpdateImageColor(newMana);
    }

    private void UpdateImageColor(float currentMana)
    {
        if (cardImage)
        {
            cardImage.color = (currentMana >= manaCost) ? Color.white : blockedColor;
        }
    }

    public void Clear()
    {
        CardDefinition = null;

        if (useVisualPresenter)
        {
            visualPresenter.Clear();
        }
        else
        {
            if (cardImage)
            {
                cardImage.sprite = null;
            }

            foreach (var orb in orbs)
            {
                orb.gameObject.SetActive(false);
            }
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
