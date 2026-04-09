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
    private TypableController typableController;
    private DeckController deckController;
    public UnityEvent<CardUI> OnCardWritten = new();
    private readonly List<InkOrb> orbs = new();

    void Awake()
    {
        typableController = GetComponentInChildren<TypableController>();
        deckController = GetComponentInParent<Player>().DeckController;
        UnityEngine.Assertions.Assert.IsNotNull(typableController);
        GetComponentInParent<Player>().CurrentMana.OnValueChanged += HandleManaChange;
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
    }

    public void UpdateCardName(ITextPipeline pipeline)
    {
        var text = pipeline.ProcessText(CardDefinition.Name);
        cardName.text = text;
        typableController.SetText(text);
    }

    public void UpdateManaCostModifier(int costModifier)
    {
        int manaCost = CardDefinition.ManaCost - deckController.GetDiscount(CardDefinition) + costModifier;
        for(int i = 0; i < orbs.Count; i++)
            orbs[i].gameObject.SetActive(i + 1 <= manaCost);
    }

    private void HandleManaChange(float prevMana, float newMana)
    {
        for (int i = 0; i < orbs.Count; i++)
        {
            bool isFilled = newMana >= i + 1;
            orbs[i].CompletelyFill(isFilled);
        }
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
