using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Card))]
public class CardUI : MonoBehaviour
{
    [field: SerializeField] public Image CardImage { get; private set; }
    [field: SerializeField] public TMP_Text CardName { get; private set; }
    [field: SerializeField] public TMP_Text CardCost { get; private set; }

    public WritableSpell WrittableSpell { get; private set; }

    public void Awake()
    {
        WrittableSpell = GetComponentInChildren<WritableSpell>();
    }

    public void SetCardInfo(CardDefinition cardDefinition)
    {
        CardImage.sprite = cardDefinition.CardImage;
        CardName.text = cardDefinition.CardName;
        CardCost.text = cardDefinition.Spell.ManaCost.ToString();
        WrittableSpell.SetText(cardDefinition.CardName);
    }
}
