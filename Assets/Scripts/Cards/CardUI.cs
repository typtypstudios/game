using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CardUI : MonoBehaviour
{
    [field: SerializeField] public CardDefinition CardDefinition { get; private set; }

    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text cardCost;
    private WritableSpell writableSpell;
    public UnityEvent<CardUI> OnCardWritten = new();

    void Awake()
    {
        writableSpell = GetComponentInChildren<WritableSpell>();
        UnityEngine.Assertions.Assert.IsNotNull(writableSpell);
    }

    void OnEnable()
    {
        writableSpell.OnSpellComplete.AddListener(OnSpellWritten);
        Player.User.PlayerInputManager.OnInputModeChangedEvent += OnInputModeChanged;
    }

    void OnDisable()
    {
        writableSpell.OnSpellComplete.RemoveListener(OnSpellWritten);
        Player.User.PlayerInputManager.OnInputModeChangedEvent -= OnInputModeChanged;
    }

    public void BindCardDefinition(CardDefinition def, int costModifier = 0)
    {
        CardDefinition = def;

        cardImage.sprite = def.CardImage;
        cardName.text = def.CardName;
        writableSpell.SetText(def.CardName);
        UpdateManaCostModifier(costModifier);
    }

    public void UpdateManaCostModifier(int costModifier)
    {
        int manaCost = CardDefinition.ManaCost + costModifier;
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

    void OnInputModeChanged(InputMode mode)
    {
        writableSpell.ToggleListener(mode == InputMode.CastingSpells);
    }
}