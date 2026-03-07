using UnityEngine;
using System;
public class Card : MonoBehaviour
{
    public static Action<CardDefinition> OnCardPlayed;

    WritableSpell spellInput;

    // public void Awake()
    // {
    //     spellInput = GetComponentInChildren<WritableSpell>();
    // }

    // public void OnEnable()
    // {
    //     spellInput.OnSpellComplete.AddListener(OnSpellComplete);
    // }

    // public void OnDisable()
    // {
    //     spellInput.OnSpellComplete.RemoveListener(OnSpellComplete);
    // }

    // void OnSpellComplete()
    // {
        
    // }
}