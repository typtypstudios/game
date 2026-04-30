using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DeckBuilder))]
public class DeckBuilderSorter : MonoBehaviour
{ 
    [SerializeField] private float interpolationTime = 0.2f;
    [SerializeField] private Toggle keepSortedToggle;
    private DeckBuilder deckBuilder;
    private AdaptiveGridLayout[] layouts;
    private readonly Dictionary<BuilderDisplayer, BuilderInitPos> initPositions = new();
    private readonly HashSet<BuilderDisplayer> displayersToReset = new();
    private readonly Dictionary<BuilderDisplayer, Transform> initParents = new();

    private struct BuilderInitPos
    {
        public BuilderInitPos(BuilderDisplayer displayer)
        {
            pos = displayer.transform.position;
            rot = displayer.transform.rotation;
        }
        public Vector3 pos;
        public Quaternion rot;
    };

    private void Awake()
    {
        layouts = GetComponentsInChildren<AdaptiveGridLayout>();
        deckBuilder = GetComponent<DeckBuilder>();
        bool keepSorted = PlayerPrefs.GetInt("keepSorted", 1) == 1;
        keepSortedToggle.SetIsOnWithoutNotify(keepSorted);
        deckBuilder.SortOnChange = keepSorted;
        deckBuilder.OnRebuild += () =>
        {
            initPositions.Clear();
            displayersToReset.Clear();
            initParents.Clear();
        };
    }

    public void OnToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt("keepSorted", isOn ? 1 : 0);
        deckBuilder.SortOnChange = isOn;
        if (isOn) SortCards(deckBuilder.UnequippedCards);
    }

    public void ReplaceCards(BuilderDisplayer card_1, BuilderDisplayer card_2)
    {
        RegisterBuilders(new[]{ card_1, card_2}, true);
        card_1.transform.SetPositionAndRotation(initPositions[card_2].pos, initPositions[card_2].rot);
        card_2.transform.SetPositionAndRotation(initPositions[card_1].pos, initPositions[card_1].rot);
        displayersToReset.Add(card_1);
        displayersToReset.Add(card_2);
        CardDefinition c = card_1.Card;
        card_1.SetInfo(card_2.Card);
        card_2.SetInfo(c);
        Transform parentCanvas = GetComponentInParent<Canvas>().transform;
        card_1.transform.SetParent(parentCanvas, true);
        card_1.transform.SetAsLastSibling();
        card_2.transform.SetParent(parentCanvas, true);
        card_2.transform.SetAsLastSibling();
        StopAllCoroutines();
        StartCoroutine(ResetPositionsCoroutine());
    }
        
    public void SortCards(List<BuilderDisplayer> displayers, bool displayAnimation = true)
    {
        List<CardDefinition> sortedCards = displayers
            .Select(d => d.Card)
            .Distinct()
            .OrderBy(c => c.Cult != null)
            .ThenBy(c => c.RequiredLevel)
            .ThenBy(c => c.Name)
            .ToList();

        if (!displayAnimation)
        {
            ApplySort(displayers, sortedCards);
            return;
        }

        RegisterBuilders(displayers);

        Dictionary<CardDefinition, int> indexDictionary = displayers
            .Select((displayer, index) => new { displayer, index })
            .ToDictionary(x => x.displayer.Card, x => x.index);

        List<(Vector3 pos, Quaternion rot)> startPositions = sortedCards
            .Select(c => (displayers[indexDictionary[c]].transform.position,
                          displayers[indexDictionary[c]].transform.rotation))
            .ToList();

        ApplySort(displayers, sortedCards);

        for(int i = 0;  i < displayers.Count; i++)
        {
            (Vector3 pos, Quaternion rot) = startPositions[i];
            displayers[i].transform.SetPositionAndRotation(pos, rot);
            displayersToReset.Add(displayers[i]);
        }

        StopAllCoroutines();
        StartCoroutine(ResetPositionsCoroutine());
    }

    private void RegisterBuilders(IEnumerable<BuilderDisplayer> displayers, bool registerParents = false)
    {
        foreach(var b in displayers)
        {
            if (!initPositions.ContainsKey(b))
                initPositions.Add(b, new BuilderInitPos(b));
            if(registerParents && !initParents.ContainsKey(b))
            {
                initParents.Add(b, b.transform.parent);
            }
        }
    }

    private void ApplySort(List<BuilderDisplayer> displayers, List<CardDefinition> sortedCards)
    {
        for (int i = 0; i < displayers.Count; i++)
        {
            displayers[i].SetInfo(sortedCards[i]);
        }
    }

    private bool MoveTowardsInitPos(BuilderDisplayer displayer, float speed, float rotSpeed)
    {
        BuilderInitPos init = initPositions[displayer];
        displayer.transform.position = Vector3.MoveTowards(displayer.transform.position, init.pos, speed * Time.deltaTime);
        displayer.transform.rotation = Quaternion.RotateTowards(displayer.transform.rotation, init.rot, rotSpeed * Time.deltaTime);
        return displayer.transform.position == init.pos && displayer.transform.rotation == init.rot;
    }

    private void ResetParents()
    {
        foreach(var displayer in initParents.Keys)
        {
            displayer.transform.SetParent(initParents[displayer], true);
        }
    }

    IEnumerator ResetPositionsCoroutine()
    {
        foreach (var layout in layouts) layout.enabled = false;
        Dictionary<BuilderDisplayer, float> speeds = new();
        Dictionary<BuilderDisplayer, float> rotSpeeds = new();
        foreach(var displayer in displayersToReset)
        {
            speeds[displayer] = Vector3.Distance(displayer.transform.position, initPositions[displayer].pos) / interpolationTime;
            rotSpeeds[displayer] = Quaternion.Angle(displayer.transform.rotation, initPositions[displayer].rot) / interpolationTime;
        }
        int placedCount = 0;
        while(placedCount < displayersToReset.Count)
        {
            placedCount = 0;
            foreach(var displayer in displayersToReset)
            {
                if (MoveTowardsInitPos(displayer, speeds[displayer], rotSpeeds[displayer])) placedCount++;
            }
            yield return null;
        }
        displayersToReset.Clear();
        ResetParents();
        foreach (var layout in layouts) layout.enabled = true;
    }
}
