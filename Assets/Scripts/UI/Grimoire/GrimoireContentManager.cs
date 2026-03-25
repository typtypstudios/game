using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireContentManager : MonoBehaviour
{
    [SerializeField] private GameObject displayerPrefab;
    [SerializeField] private int numDisplayers = 9;
    [SerializeField] private float transitionSpeed = 1;
    [SerializeField] private GrimoireSection[] sections;
    private GrimoireNavigationController navController;
    private GrimoireInfoPanel infoPanel;
    private CanvasGroup canvasGroup;
    private readonly List<GrimoireInfoDisplayer> displayers = new();
    private readonly List<Page> pages = new();
    private int currentPage = 0;
    private readonly List<int> sectionStartPages = new();
    private int currentSection = 0;
    public event Action<int, int> OnPageChanged;
    public event Action<int, int> OnSectionChanged;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        navController = GetComponentInParent<GrimoireNavigationController>();
        infoPanel = FindFirstObjectByType<GrimoireInfoPanel>();
        for(int i = 0; i < numDisplayers; i++)
            displayers.Add(Instantiate(displayerPrefab, this.transform).GetComponent<GrimoireInfoDisplayer>());
    }

    private void Start() => InitializeData();

    private void InitializeData()
    {
        foreach (var section in sections) GenerateSection(section);
        StartCoroutine(TurnPageCoroutine(true));
        displayers[0].GetComponent<Button>().onClick?.Invoke();
    }

    public void GoToPage(int idx)
    {
        idx = Mathf.Clamp(idx, 0, pages.Count - 1);
        int prevSection = currentSection;
        currentSection = pages[idx].sectionIndex;
        OnSectionChanged?.Invoke(currentSection, prevSection);
        currentPage = idx;
        OnPageChanged?.Invoke(currentPage, pages.Count);
        StopAllCoroutines();
        StartCoroutine(TurnPageCoroutine());
    }

    public void TurnPage(int pages) => GoToPage(currentPage + pages);

    public void GoToSection(int idx)
    {
        idx = Mathf.Clamp(idx, 0, sectionStartPages.Count - 1);
        GoToPage(sectionStartPages[idx]);
    }

    private void GenerateSection(GrimoireSection section)
    {
        navController.AddSection(sectionStartPages.Count, section.sectionName);
        ADefinition[] definitions;
        if (section.cardRegister) 
            definitions = section.cardRegister.RegisteredItems.OrderBy(c => c.Name).ToArray();
        else definitions = section.effectRegister.RegisteredItems.OrderBy(c => c.name).ToArray();
        FillSection(definitions);
    }

    private void FillSection(ADefinition[] definition)
    {
        sectionStartPages.Add(pages.Count);
        for(int i = 0; i < definition.Length; i += numDisplayers)
        {
            Page currentPage = new(pages.Count, sectionStartPages.Count - 1);
            for (int j = 0; j < numDisplayers; j++)
            {
                int idx = i + j;
                if (idx >= definition.Length) break;
                currentPage.definition.Add(definition[idx]);
            }
            pages.Add(currentPage);
        }
    }

    private void ResetDisplayers()
    {
        foreach (var displayer in displayers)
        {
            displayer.GetComponent<WritableButton>().ResetButton(true);
            displayer.Highlight(false);
        }
    }

    private void BlockDisplayers(bool block)
    {
        foreach(var displayer in displayers)
        {
            displayer.GetComponent<WritableButton>().CompletelyBlock(block);
        }
    }

    IEnumerator TurnPageCoroutine(bool directTransition = false)
    {
        BlockDisplayers(true);
        while(canvasGroup.alpha > 0 && !directTransition)
        {
            canvasGroup.alpha -= transitionSpeed * Time.deltaTime;
            yield return null;
        }
        ResetDisplayers();
        for (int i = 0; i < numDisplayers; i++)
        {
            if (i < pages[currentPage].definition.Count)
            {
                displayers[i].gameObject.SetActive(true);
                displayers[i].SetInfo(pages[currentPage].definition[i]);
                if (displayers[i].Definition.Name.Equals(infoPanel.DisplayedName)) 
                    displayers[i].Highlight(true);
            }
            else displayers[i].gameObject.SetActive(false);
        }
        while (canvasGroup.alpha < 1 && !directTransition)
        {
            canvasGroup.alpha += transitionSpeed * Time.deltaTime;
            yield return null;
        }
        BlockDisplayers(false);
    }
}

[Serializable]
public class GrimoireSection
{
    public string sectionName;
    public CardRegister cardRegister;
    public StatusEffectRegister effectRegister;
}

public struct Page
{
    public int pageIndex;
    public int sectionIndex;
    public List<ADefinition> definition;

    public Page(int idx, int sectionIdx)
    {
        pageIndex = idx;
        sectionIndex = sectionIdx;
        definition = new();
    }
}