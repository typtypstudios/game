using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TypTyp.TextSystem;

public class CardUI : MonoBehaviour
{
    [field: SerializeField] public CardDefinition CardDefinition { get; private set; }

    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text cardCost;
    private WritableSpell writableSpell;
    private DeckController deckController;
    public UnityEvent<CardUI> OnCardWritten = new();

    void Awake()
    {
        writableSpell = GetComponentInChildren<WritableSpell>();
        deckController = GetComponentInParent<Player>().DeckController;
        UnityEngine.Assertions.Assert.IsNotNull(writableSpell);
    }

    void OnEnable()
    {
        writableSpell.OnSpellComplete.AddListener(OnSpellWritten);
    }

    void OnDisable()
    {
        writableSpell.OnSpellComplete.RemoveListener(OnSpellWritten);
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
        writableSpell.SetText(text);
    }

    public void UpdateManaCostModifier(int costModifier)
    {
        int manaCost = CardDefinition.ManaCost - deckController.GetDiscount(CardDefinition) + costModifier;
        cardCost.text = manaCost.ToString();
    }

    public void Clear()
    {
        CardDefinition = null;
        cardImage.sprite = null;
        cardName.text = string.Empty;
        cardCost.text = string.Empty;
    }

    void OnSpellWritten(string _)
    {
        OnCardWritten?.Invoke(this);
    }

    // El modo de input se gestiona via TypingInputListener en el propio objeto.
}
