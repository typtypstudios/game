using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireContentManager : MonoBehaviour
{
    [SerializeField] private float transitionSpeed = 1;
    private GrimoireInfoDisplayer[] displayers;
    private GrimoireInfoPanel infoPanel;
    private CanvasGroup canvasGroup;
    public List<GrimoirePage> Pages { get; set; } = new();
    private int currentPage = 0;
    public List<int> SectionStartPages { get; set; } = new();
    private int currentSection = 0;
    public event Action<int, int> OnPageChanged;
    public event Action<int, int> OnSectionChanged;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        infoPanel = FindFirstObjectByType<GrimoireInfoPanel>();
        displayers = GetComponentsInChildren<GrimoireInfoDisplayer>();
    }

    private void Start()
    {
        StartCoroutine(TurnPageCoroutine(true));
        OnSectionChanged?.Invoke(currentSection, currentSection);
        OnPageChanged?.Invoke(currentPage, Pages.Count);
        displayers[0].GetComponent<Button>().onClick?.Invoke();
    }

    public void GoToPage(int idx)
    {
        idx = Mathf.Clamp(idx, 0, Pages.Count - 1);
        int prevSection = currentSection;
        currentSection = Pages[idx].sectionIndex;
        OnSectionChanged?.Invoke(currentSection, prevSection);
        currentPage = idx;
        OnPageChanged?.Invoke(currentPage, Pages.Count);
        StopAllCoroutines();
        StartCoroutine(TurnPageCoroutine());
    }

    public void TurnPage(int pages) => GoToPage(currentPage + pages);

    public void GoToSection(int idx)
    {
        idx = Mathf.Clamp(idx, 0, SectionStartPages.Count - 1);
        GoToPage(SectionStartPages[idx]);
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

    IEnumerator TurnPageCoroutine(bool directTransition = false)
    {
        BlockDisplayers(true);
        while(canvasGroup.alpha > 0 && !directTransition)
        {
            canvasGroup.alpha -= transitionSpeed * Time.deltaTime;
            yield return null;
        }
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
        while (canvasGroup.alpha < 1 && !directTransition)
        {
            canvasGroup.alpha += transitionSpeed * Time.deltaTime;
            yield return null;
        }
        BlockDisplayers(false);
    }
}