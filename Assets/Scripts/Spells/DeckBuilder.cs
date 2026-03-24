using System.Collections;
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
    private readonly List<CardDisplayer_builder> equippedCards = new();
    private readonly List<CardDisplayer_builder> unequippedCards = new();
    private List<int> equippedIndexes = new();
    private CardDisplayer_builder selectedEquipped;
    private CardDisplayer_builder selectedUnequipped;
    public static CardDefinition[] CardsInDeck { get; private set; }

    private void Awake()
    {
        LoadEquippedCards();
        CardsInDeck = equippedCards.Select(c => c.Card).ToArray();
        LoadUnequippedCards();
        foreach (GameObject panel in highlightPanels) 
            panel.transform.SetAsLastSibling(); //Para estar por encima de las cartas creadas
        CardDisplayer_builder.OnCardChosen += ProcessCardChosen;
    }

    private void OnDestroy() => CardDisplayer_builder.OnCardChosen -= ProcessCardChosen;

    public void SaveEquippedCards()
    {
        List<int> indexes = equippedCards.Select(c => CardRegister.Instance.GetId(c.Card)).ToList();
        string eCardsString = string.Join(",", indexes);
        PlayerPrefsEncoder.SetString("EquippedCards", eCardsString);
        CardsInDeck = equippedCards.Select(c => c.Card).ToArray();
    }

    public void ResetSelection()
    {
        if (selectedEquipped) selectedEquipped.Highlight(false);
        if (selectedUnequipped) selectedUnequipped.Highlight(false);
        selectedEquipped = null;
        selectedUnequipped = null;
        foreach (GameObject panel in highlightPanels) panel.SetActive(false);
    }

    private void LoadEquippedCards()
    {
        //Creaci�n de cartas:
        for (int i = 0; i < Settings.Instance.DeckSize; i++)
        {
            CardDisplayer_builder c = Instantiate(cardPrefab, equippedLayout).GetComponent<CardDisplayer_builder>();
            equippedCards.Add(c);
        }
        //Carga de cartas guardadas:
        equippedIndexes = Enumerable.Range(0, Settings.Instance.DeckSize).ToList();
        string eCardsString = PlayerPrefsEncoder.GetString("EquippedCards", string.Empty);
        if (!eCardsString.Equals(string.Empty))
            equippedIndexes = eCardsString.Split(',').Select(int.Parse).ToList();
        //Manejo de disminuci�n de deck size:
        if (Settings.Instance.DeckSize < equippedIndexes.Count)
            equippedIndexes.RemoveRange(Settings.Instance.DeckSize - 1,
                equippedIndexes.Count - Settings.Instance.DeckSize);
        //Configuraci�n de cartas:
        for (int i = 0; i < equippedIndexes.Count; i++)
                equippedCards[i].SetCard(CardRegister.Instance.GetById(equippedIndexes[i]));
        //Manejo de aumento de deck size:
        if(equippedIndexes.Count < equippedCards.Count)
        {
            int count = equippedCards.Count - equippedIndexes.Count;
            for(int i = 0; i < count; i++)
            {
                List<CardDefinition> currentCards = equippedCards.Select(c => c.Card).ToList();
                (CardDefinition c, int idx) = CardRegister.Instance.GetUncontainedItem(currentCards);
                equippedCards[equippedIndexes.Count].SetCard(c);
                equippedIndexes.Add(idx);
            }
        }
    }

    private void LoadUnequippedCards()
    {
        for(int i = 0; i < CardRegister.Instance.Count; i++)
        {
            if (equippedIndexes.Contains(i)) continue;
            CardDisplayer_builder c = Instantiate(cardPrefab, unequippedLayout).GetComponent<CardDisplayer_builder>();
            unequippedCards.Add(c);
            c.SetCard(CardRegister.Instance.GetById(i));
        }
    }

    private void ProcessCardChosen(CardDisplayer_builder card)
    {
        StartCoroutine(ResetButtonsCoroutine()); //Se resetean los botones para resetear su escritura
        if (card == selectedEquipped || card == selectedUnequipped)
        {
            ResetSelection();
            return;
        }
        if(card.transform.parent == equippedLayout)
        {
            selectedEquipped?.Highlight(false);
            selectedEquipped = card;
        }
        else
        {
            selectedUnequipped?.Highlight(false);
            selectedUnequipped = card;
        }
        card.Highlight(true); //Lo pone como �ltimo hijo, delante de highlight panel
        CheckCardChange();
        //Si alg�n objeto est� en hover (ser� �ltimo hijo) el panel se activa
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

    IEnumerator ResetButtonsCoroutine()
    {
        yield return null;
        foreach (var b in GetComponentsInChildren<WritableButton>()) b.ResetButton();
    }
}
