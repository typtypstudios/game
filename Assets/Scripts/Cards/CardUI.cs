using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TypTyp.TextSystem;
using TypTyp.TextSystem.Typable;
using System.Collections.Generic;
using TypTyp;

public class CardUI : MonoBehaviour
{
    [field: SerializeField] public CardDefinition CardDefinition { get; private set; }

    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private Transform cardCostLayout;
    [SerializeField] private GameObject inkOrbPrefab;
    [SerializeField] private Color blockedColor = Color.grey;
    private TypableController typableController;
    private DeckController deckController;
    public UnityEvent<CardUI> OnCardWritten = new();
    private readonly List<InkOrb> orbs = new();
    private int manaCost;
    private Player player;

    void Awake()
    {
        typableController = GetComponentInChildren<TypableController>();
        deckController = GetComponentInParent<Player>().DeckController;
        UnityEngine.Assertions.Assert.IsNotNull(typableController);
        player = GetComponentInParent<Player>();
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
    }

    void OnDisable()
    {
        typableController.OnComplete -= OnSpellWritten;
    }

    public void BindCardDefinition(CardDefinition def, ITextPipeline pipeline, int costModifier = 0)
    {
        CardDefinition = def;

        cardImage.sprite = def.Image;
        UpdateCardName(pipeline);
        UpdateManaCostModifier(costModifier);
        UpdateImageColor(player.CurrentMana.Value);
    }

    public void UpdateCardName(ITextPipeline pipeline)
    {
        var text = pipeline.ProcessText(CardDefinition.Name);
        cardName.text = text;
        typableController.SetText(text);
    }

    public void UpdateManaCostModifier(int costModifier)
    {
        manaCost = CardDefinition.ManaCost - deckController.GetDiscount(CardDefinition) + costModifier;
        for(int i = 0; i < orbs.Count; i++)
            orbs[i].gameObject.SetActive(i + 1 <= manaCost);
        UpdateImageColor(player.CurrentMana.Value);
    }

    private void HandleManaChange(float prevMana, float newMana)
    {
        for (int i = 0; i < orbs.Count; i++)
        {
            bool isFilled = newMana >= i + 1;
            orbs[i].CompletelyFill(isFilled);
        }
        UpdateImageColor(newMana);
    }

    private void UpdateImageColor(float currentMana)
    {
        cardImage.color = (currentMana >= manaCost) ? Color.white : blockedColor;
    }

    public void Clear()
    {
        CardDefinition = null;
        cardImage.sprite = null;
        cardName.text = string.Empty;
        foreach(var orb in orbs) orb.gameObject.SetActive(false);
    }

    void OnSpellWritten()
    {
        OnCardWritten?.Invoke(this);
    }

    // El modo de input se gestiona via TypingInputListener en el propio objeto.
}
