using System.Collections.Generic;
using System.Linq;
using TypTyp;
using UnityEngine;

public class DeckBuilder : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform equippedLayout;
    [SerializeField] private Transform unequippedLayout;
    [SerializeField] private GameObject[] highlightPanels;
    private readonly List<CardUI_Builder> equippedCards = new();
    private readonly List<CardUI_Builder> unequippedCards = new();
    private List<int> equippedIndexes = new();
    private CardUI_Builder selectedEquipped;
    private CardUI_Builder selectedUnequipped;

    private void Awake()
    {
        LoadEquippedCards();
        LoadUnequippedCards();
        foreach (GameObject panel in highlightPanels) panel.transform.SetAsLastSibling();
        CardUI_Builder.OnCardChosen += ProcessCardChosen;
    }

    private void OnDestroy() => CardUI_Builder.OnCardChosen -= ProcessCardChosen;

    public void SaveEquippedCards()
    {
        string eCardsString = string.Join(",", equippedCards.Select(c => CardRegister.Instance.GetId(c.Card)));
        PlayerPrefs.SetString("EquippedCards", eCardsString);
    }

    public void ResetSelection()
    {
        if (selectedEquipped) selectedEquipped.SetHover(false);
        if (selectedUnequipped) selectedUnequipped.SetHover(false);
        selectedEquipped = null;
        selectedUnequipped = null;
        foreach (GameObject panel in highlightPanels) panel.SetActive(false);
    }

    private void LoadEquippedCards()
    {
        //Creación de cartas:
        for (int i = 0; i < Settings.Instance.DeckSize; i++)
        {
            CardUI_Builder c = Instantiate(cardPrefab, equippedLayout).GetComponent<CardUI_Builder>();
            equippedCards.Add(c);
        }
        //Carga de cartas guardadas
        string eCardsString = PlayerPrefs.GetString("EquippedCards", string.Empty);
        equippedIndexes = Enumerable.Range(0, Settings.Instance.DeckSize).ToList();
        if(!eCardsString.Equals(string.Empty))
            equippedIndexes = eCardsString.Split(',').Select(int.Parse).ToList();
        //Configuración de cartas:
        for (int i = 0; i < equippedCards.Count; i++)
            equippedCards[i].SetCard(CardRegister.Instance.GetById(equippedIndexes[i]));
    }

    private void LoadUnequippedCards()
    {
        for(int i = 0; i < CardRegister.Instance.Count; i++)
        {
            if (equippedIndexes.Contains(i)) continue;
            CardUI_Builder c = Instantiate(cardPrefab, unequippedLayout).GetComponent<CardUI_Builder>();
            unequippedCards.Add(c);
            c.SetCard(CardRegister.Instance.GetById(i));
        }
    }

    private void ProcessCardChosen(CardUI_Builder card)
    {
        if(card == selectedEquipped || card == selectedUnequipped)
        {
            ResetSelection();
            return;
        }
        if(card.transform.parent == equippedLayout)
        {
            if (selectedEquipped) selectedEquipped.SetHover(false);
            selectedEquipped = card;
        }
        else
        {
            if(selectedUnequipped) selectedUnequipped.SetHover(false);
            selectedUnequipped = card;
        }
        card.SetHover(true); //Lo pone como último hijo, delante de highlight panel
        CheckCardChange();
        foreach (GameObject panel in highlightPanels) 
            panel.SetActive(panel.transform.GetSiblingIndex() != panel.transform.parent.childCount - 1);
    }

    private void CheckCardChange()
    {
        if (selectedUnequipped && selectedEquipped)
        {
            CardDefinition c = selectedUnequipped.Card;
            selectedUnequipped.SetCard(selectedEquipped.Card);
            selectedEquipped.SetCard(c);
            ResetSelection();
        }
    }
}
