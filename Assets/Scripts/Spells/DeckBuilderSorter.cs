using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(DeckBuilder))]
public class DeckBuilderSorter : MonoBehaviour
{
    [SerializeField] private float sortTime = 0.5f;
    private AdaptiveGridLayout[] layouts;

    private void Awake()
    {
        layouts = GetComponentsInChildren<AdaptiveGridLayout>();
    }

    public void SortCards(List<BuilderDisplayer> cards, bool displayAnimation = true)
    {
        List<CardDefinition> sortedCards = cards
            .Select(d => d.Card)
            .Distinct()
            .OrderBy(c => c.Cult != null)
            .ThenBy(c => c.RequiredLevel)
            .ThenBy(c => c.Name)
            .ToList();

        if (!displayAnimation)
        {
            ApplySort(cards, sortedCards);
            return;
        }

        Dictionary<CardDefinition, int> indexDictionary = sortedCards
            .Select((card, index) => new { card, index })
            .ToDictionary(x => x.card, x => x.index);

        List<Vector2> destinations = cards
            .Select(c => cards[indexDictionary[c.Card]].GetComponent<RectTransform>().anchoredPosition)
            .ToList();

        StopAllCoroutines();
        StartCoroutine(SortCoroutine(cards, sortedCards, destinations));
    }

    private void ApplySort(List<BuilderDisplayer> cards, List<CardDefinition> sortedCards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].SetInfo(sortedCards[i]);
        }
    }

    //Código muy mejorable, pero funciona. Si tengo tiempo en algún momento lo cambio
    IEnumerator SortCoroutine(List<BuilderDisplayer> cards, List<CardDefinition> sortedCards, 
        List<Vector2> destinations)
    {
        foreach (var layout in layouts) layout.enabled = false;
        List<RectTransform> rts = new();
        List<float> speeds = new();
        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform rt = cards[i].GetComponent<RectTransform>();
            speeds.Add(Vector2.Distance(rt.anchoredPosition, destinations[i]) / sortTime);
            rts.Add(rt);
        }
        HashSet<int> finishedList = new();
        while (finishedList.Count < cards.Count)
        {
            for(int i = 0; i < cards.Count; i++)
            {
                if (rts[i].anchoredPosition == destinations[i])
                {
                    finishedList.Add(i);
                    continue;
                }
                rts[i].anchoredPosition = Vector2.MoveTowards(rts[i].anchoredPosition, destinations[i], speeds[i] * Time.deltaTime);
            }
            yield return null;
        }
        ApplySort(cards, sortedCards);
        foreach (var layout in layouts) layout.enabled = true;
    }
}
