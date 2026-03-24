using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireContentManager : MonoBehaviour //Habría que refactorizar, pero funciona
{
    [SerializeField] private GameObject displayerPrefab;
    [SerializeField] private int numDisplayers = 9;
    private readonly List<GrimoireInfoDisplayer> displayers = new();
    private CardDefinition[] defaultCards;
    private StatusEffectDefinition[] effects;
    private readonly List<Page> pages = new();
    private readonly List<int> sectionStartPages = new();
    
    private void Awake()
    {
        defaultCards = CardRegister.Instance.RegisteredItems.OrderBy(c => c.CardName).ToArray();
        effects = StatusEffectRegister.Instance.RegisteredItems.OrderBy(e => e.EffectName).ToArray();
        for(int i = 0; i < numDisplayers; i++)
            displayers.Add(Instantiate(displayerPrefab, this.transform).GetComponent<GrimoireInfoDisplayer>());
    }

    private void Start() => InitializeData();

    private void InitializeData()
    {
        AddSection(defaultCards);
        AddSection(effects);
        GoToPage(0);
        displayers[0].GetComponent<Button>().onClick?.Invoke();
    }

    public void GoToPage(int idx)
    {
        idx = Mathf.Clamp(idx, 0, pages.Count - 1);
        if (pages[idx].cards.Count > 0)
        {
            for (int i = 0; i < numDisplayers; i++)
            {
                if (pages[idx].cards.Count > 0)
                {
                    if (i < pages[idx].cards.Count)
                    {
                        displayers[i].SetCard(pages[idx].cards[i]);
                        displayers[i].gameObject.SetActive(true);
                    }
                    else displayers[i].gameObject.SetActive(false);
                }
                else
                {
                    if (i < pages[idx].effects.Count)
                    {
                        displayers[i].SetEffect(pages[idx].effects[i]);
                        displayers[i].gameObject.SetActive(true);
                    }
                    else displayers[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void GoToSection(int idx)
    {
        idx = Mathf.Clamp(idx, 0, sectionStartPages.Count - 1);
        GoToPage(sectionStartPages[idx]);
    }

    private void AddSection(CardDefinition[] cards)
    {
        sectionStartPages.Add(pages.Count);
        for(int i = 0; i < cards.Length; i += numDisplayers)
        {
            Page currentPage = new(pages.Count, sectionStartPages.Count);
            for (int j = 0; j < numDisplayers; j++)
            {
                int idx = i + j;
                if (idx >= cards.Length) break;
                currentPage.cards.Add(cards[idx]);
            }
            pages.Add(currentPage);
        }
    }

    private void AddSection(StatusEffectDefinition[] effects)
    {
        sectionStartPages.Add(pages.Count);
        for (int i = 0; i < effects.Length; i += numDisplayers)
        {
            Page currentPage = new(pages.Count, sectionStartPages.Count);
            for (int j = 0; j < numDisplayers; j++)
            {
                int idx = i + j;
                if (idx >= effects.Length) break;
                currentPage.effects.Add(effects[idx]);
            }
            pages.Add(currentPage);
        }
    }
}

public struct Page
{
    public int pageIndex;
    public int sectionIndex;
    public List<CardDefinition> cards;
    public List<StatusEffectDefinition> effects;

    public Page(int idx, int sectionIdx)
    {
        pageIndex = idx;
        sectionIndex = sectionIdx;
        cards = new();
        effects = new();
    }
}