using System;
using System.Collections.Generic;
using UnityEngine;

public class GrimoireContentManager : MonoBehaviour
{
    [SerializeField] private GrimoireInfoPanel infoPanel;
    private GrimoireInfoDisplayer[] displayers;
    private TurnPageEffect turnPageEffect;
    public List<GrimoirePage> Pages { get; set; } = new();
    private int currentPage = 0;
    public List<int> SectionStartPages { get; set; } = new();
    private int currentSection = 0;
    public event Action<int, int> OnPageChanged;
    public event Action<int, int> OnSectionChanged;

    private void Start()
    {
        if (!TryGetComponent(out turnPageEffect))
            Debug.LogError("Error: no hay efecto de cambio de página (componente Turn Page Effect)");
        turnPageEffect.OnBlankPage += UpdateContent;
        turnPageEffect.OnTurnFinished += () => BlockDisplayers(false);
        displayers = GetComponentsInChildren<GrimoireInfoDisplayer>();
        foreach (var displayer in displayers) displayer.Initialize(infoPanel);
        GoToPage(0, false);
        displayers[0].PerformClick();
    }

    public void GoToPage(int idx, bool performTransition = true)
    {
        idx = Mathf.Clamp(idx, 0, Pages.Count - 1);
        int prevSection = currentSection;
        currentSection = Pages[idx].sectionIndex;
        OnSectionChanged?.Invoke(currentSection, prevSection);
        currentPage = idx;
        OnPageChanged?.Invoke(currentPage, Pages.Count);
        if (performTransition)
        {
            BlockDisplayers(true);
            turnPageEffect.TurnPage();
        }
        else UpdateContent();
    }

    public void TurnPage(int pages) => GoToPage(currentPage + pages);

    public void GoToSection(int idx)
    {
        idx = Mathf.Clamp(idx, 0, SectionStartPages.Count - 1);
        GoToPage(SectionStartPages[idx]);
    }

    public void GoToDefinition(string definitionName)
    {
        foreach(var page in Pages)
        {
            foreach(var def in page.definitions)
            {
                if(def.Name.Equals(definitionName))
                {
                    GoToPage(page.pageIndex, false);
                    displayers[page.definitions.IndexOf(def)].PerformClick();
                    return;
                }
            }
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
            displayer.GetComponent<WritableButton>().CompletelyBlock(block);
    }

    private void UpdateContent()
    {
        ResetDisplayers();
        for (int i = 0; i < displayers.Length; i++)
        {
            if (i < Pages[currentPage].definitions.Count)
            {
                displayers[i].gameObject.SetActive(true);
                displayers[i].SetInfo(Pages[currentPage].definitions[i]);
                if (displayers[i].Definition.Name.Equals(infoPanel.DisplayedName))
                    displayers[i].Highlight(true);
            }
            else displayers[i].gameObject.SetActive(false);
        }
    }
}