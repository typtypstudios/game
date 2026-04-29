using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(DeckBuilder))]
public class DeckBuilderSorter : MonoBehaviour
{
    [SerializeField] private float interpolationTime = 0.2f;
    private AdaptiveGridLayout[] layouts;

    private void Awake()
    {
        layouts = GetComponentsInChildren<AdaptiveGridLayout>();
    }

    public void ReplaceCards(BuilderDisplayer card_1, BuilderDisplayer card_2)
    {
        StopAllCoroutines();
        StartCoroutine(ReplaceCardsCoroutine(card_1, card_2));
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
        StartCoroutine(SortCardsCoroutine(cards, sortedCards, destinations));
    }

    private void ApplySort(List<BuilderDisplayer> cards, List<CardDefinition> sortedCards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].SetInfo(sortedCards[i]);
        }
    }

    //Código muy mejorable, pero funciona. Si tengo tiempo en algún momento lo cambio
    IEnumerator SortCardsCoroutine(List<BuilderDisplayer> cards, List<CardDefinition> sortedCards, 
        List<Vector2> destinations)
    {
        foreach (var layout in layouts) layout.enabled = false;
        List<RectTransform> rts = new();
        List<float> speeds = new();
        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform rt = cards[i].GetComponent<RectTransform>();
            speeds.Add(Vector2.Distance(rt.anchoredPosition, destinations[i]) / interpolationTime);
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

    IEnumerator ReplaceCardsCoroutine(BuilderDisplayer card_1, BuilderDisplayer card_2)
    {
        foreach (var layout in layouts) layout.enabled = false;
        RectTransform rt_1 = card_1.GetComponent<RectTransform>();
        RectTransform rt_2 = card_2.GetComponent<RectTransform>();
        Vector3 dest_1 = rt_2.position;
        Quaternion dest_rot_1 = rt_2.rotation;
        Quaternion init_rot_1 = rt_1.rotation;
        Vector3 dest_2 = rt_1.position;
        Quaternion dest_rot_2 = rt_1.rotation;
        Quaternion init_rot_2 = rt_2.rotation;
        float speed = Vector3.Distance(rt_1.position, rt_2.position) / interpolationTime;
        float rotSpeed = Quaternion.Angle(rt_1.rotation, rt_2.rotation) / interpolationTime;
        while (rt_1.position != dest_1 && rt_2.position != dest_2)
        {
            rt_1.position = Vector3.MoveTowards(rt_1.position, dest_1, speed * Time.deltaTime);
            rt_1.rotation = Quaternion.RotateTowards(rt_1.rotation, dest_rot_1, rotSpeed * Time.deltaTime);
            rt_2.position = Vector3.MoveTowards(rt_2.position, dest_2, speed * Time.deltaTime);
            rt_2.rotation = Quaternion.RotateTowards(rt_2.rotation, dest_rot_2, rotSpeed * Time.deltaTime);
            yield return null;
        }
        CardDefinition c = card_1.Card;
        card_1.SetInfo(card_2.Card);
        card_2.SetInfo(c);
        rt_1.rotation = init_rot_1;
        rt_2.rotation = init_rot_2;
        foreach (var layout in layouts) layout.enabled = true;
    }
}
