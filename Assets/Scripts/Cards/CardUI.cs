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
    }

    void OnEnable()
    {
        writableSpell.OnSpellComplete.AddListener(OnSpellWritten);
    }

    void OnDisable()
    {
        writableSpell.OnSpellComplete.RemoveListener(OnSpellWritten);
    }

    public void BindCardDefinition(CardDefinition def)
    {
        CardDefinition = def;

        cardImage.sprite = def.CardImage;
        cardName.text = def.CardName;
        cardCost.text = def.Spell.ManaCost.ToString();
        writableSpell.SetText(def.CardName);
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
}