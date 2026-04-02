using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TypTyp;
using UnityEngine;

//?

public class DeckBuilder : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform equippedLayout;
    [SerializeField] private Transform unequippedLayout;
    [SerializeField] private GameObject[] highlightPanels;
    private readonly List<BuilderDisplayer> equippedCards = new();
    private readonly List<BuilderDisplayer> unequippedCards = new();
    private List<int> equippedIndexes = new();
    private BuilderDisplayer selectedEquipped;
    private BuilderDisplayer selectedUnequipped;
    public static CardDefinition[] CardsInDeck { get; private set; }

    private void OnEnable()
    {
        SaveManager.Instance.OnBeforeSave += HandleBeforeSave;
        SaveManager.Instance.OnAfterLoad += HandleAfterLoad;
    }

    private void Awake()
    {
        InitializeEquippedCards();

        if (SaveManager.Instance.HasLoadedState)
        {
            SaveState state = SaveManager.Instance.GetState();
            ApplyDeck(state);
        }
        else
        {
            ApplyDeck(new SaveState());
        }

        foreach (GameObject panel in highlightPanels)
        {
            // Keep highlight panels behind cards so selected cards remain visible
            // without reordering card siblings.
            panel.transform.SetAsFirstSibling();
        }

        BuilderDisplayer.OnCardChosen += ProcessCardChosen;
    }

    private void OnDisable()
    {
        if (SaveManager.Instance == null) return;

        SaveManager.Instance.OnBeforeSave -= HandleBeforeSave;
        SaveManager.Instance.OnAfterLoad -= HandleAfterLoad;
    }

    private void OnDestroy()
    {
        BuilderDisplayer.OnCardChosen -= ProcessCardChosen;
    }

    public void SaveEquippedCards()
    {
        RefreshCardsInDeck();
        SaveManager.Instance.Save();
    }

    public void ResetSelection()
    {
        if (selectedEquipped) selectedEquipped.Highlight(false);
        if (selectedUnequipped) selectedUnequipped.Highlight(false);
        selectedEquipped = null;
        selectedUnequipped = null;
        UpdateHighlightPanelsVisibility();
    }

    private void InitializeEquippedCards()
    {
        for (int i = 0; i < Settings.Instance.DeckSize; i++)
        {
            BuilderDisplayer card = Instantiate(cardPrefab, equippedLayout).GetComponent<BuilderDisplayer>();
            equippedCards.Add(card);
        }
    }

    private void RebuildUnequippedCards()
    {
        foreach (BuilderDisplayer card in unequippedCards)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }

        unequippedCards.Clear();

        for (int i = 0; i < CardRegister.Instance.Count; i++)
        {
            if (equippedIndexes.Contains(i)) continue;
            BuilderDisplayer card = Instantiate(cardPrefab, unequippedLayout).GetComponent<BuilderDisplayer>();
            unequippedCards.Add(card);
            card.SetInfo(CardRegister.Instance.GetById(i));
        }
    }

    private void ProcessCardChosen(BuilderDisplayer card)
    {
        StartCoroutine(ResetButtonsCoroutine());
        if (card == selectedEquipped || card == selectedUnequipped)
        {
            ResetSelection();
            return;
        }

        if (card.transform.parent == equippedLayout)
        {
            selectedEquipped?.Highlight(false);
            selectedEquipped = card;
        }
        else
        {
            selectedUnequipped?.Highlight(false);
            selectedUnequipped = card;
        }

        card.Highlight(true);
        CheckCardChange();
        UpdateHighlightPanelsVisibility();
    }

    private void CheckCardChange()
    {
        if (selectedUnequipped && selectedEquipped)
        {
            CardDefinition card = selectedUnequipped.Card;
            selectedUnequipped.SetInfo(selectedEquipped.Card);
            selectedEquipped.SetInfo(card);
            RefreshIndexesFromUI();
            RefreshCardsInDeck();
            ResetSelection();
        }
    }

    private IEnumerator ResetButtonsCoroutine()
    {
        yield return null;
        foreach (WritableButton button in GetComponentsInChildren<WritableButton>()) button.ResetButton();
    }

    private void HandleBeforeSave(SaveState state)
    {
        RefreshIndexesFromUI();
        RefreshCardsInDeck();
        state.slot.deck.equippedCardIds = equippedIndexes.ToList();
    }

    private void HandleAfterLoad(SaveState state)
    {
        ApplyDeck(state);
    }

    private void ApplyDeck(SaveState state)
    {
        equippedIndexes = ResolveEquippedIndexes(state?.slot?.deck?.equippedCardIds);

        for (int i = 0; i < equippedCards.Count; i++)
        {
            equippedCards[i].SetInfo(CardRegister.Instance.GetById(equippedIndexes[i]));
        }

        RebuildUnequippedCards();
        RefreshCardsInDeck();
        ResetSelection();
    }

    private List<int> ResolveEquippedIndexes(List<int> savedIndexes)
    {
        List<int> resolved = new();
        if (savedIndexes != null)
        {
            resolved = savedIndexes
                .Where(index => index >= 0 && index < CardRegister.Instance.Count)
                .Distinct()
                .Take(Settings.Instance.DeckSize)
                .ToList();
        }

        if (resolved.Count == 0)
        {
            resolved = Enumerable.Range(0, Mathf.Min(Settings.Instance.DeckSize, CardRegister.Instance.Count)).ToList();
        }

        while (resolved.Count < equippedCards.Count)
        {
            List<CardDefinition> currentCards = resolved.Select(CardRegister.Instance.GetById).ToList();
            (CardDefinition card, int idx) = CardRegister.Instance.GetUncontainedItem(currentCards);
            resolved.Add(idx);
        }

        return resolved;
    }

    private void RefreshIndexesFromUI()
    {
        equippedIndexes = equippedCards.Select(card => CardRegister.Instance.GetId(card.Card)).ToList();
    }

    private void RefreshCardsInDeck()
    {
        CardsInDeck = equippedCards.Select(card => card.Card).ToArray();
    }

    private void UpdateHighlightPanelsVisibility()
    {
        bool hasSelection = selectedEquipped != null || selectedUnequipped != null;
        foreach (GameObject panel in highlightPanels)
        {
            panel.SetActive(hasSelection);
        }
    }
}
