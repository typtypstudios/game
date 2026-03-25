using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireNavigationController : MonoBehaviour
{
    [SerializeField] private GameObject sectionButtonPrefab;
    [SerializeField] private Transform sectionsTransform;
    [SerializeField] private WritableButton button_prev;
    [SerializeField] private WritableButton button_next;
    private readonly List<WritableButton> sections = new();
    private GrimoireContentManager contentManager;

    private void Awake()
    {
        contentManager = GetComponentInChildren<GrimoireContentManager>();
        contentManager.OnPageChanged += CheckCurrentPage;
        contentManager.OnSectionChanged += CheckCurrentSection;
    }

    private void OnDestroy()
    {
        contentManager.OnPageChanged -= CheckCurrentPage;
        contentManager.OnSectionChanged -= CheckCurrentSection;
    }

    public void AddSection(int index, string name)
    {
        WritableButton s = Instantiate(sectionButtonPrefab, sectionsTransform).GetComponent<WritableButton>();
        s.GetComponent<Button>().onClick.AddListener(() => contentManager.GoToSection(index));
        sections.Add(s);
        s.OverrideText(name);
    }

    public void CheckCurrentSection(int newIdx, int prevIdx)
    {
        foreach(var section in sections) section.ResetButton();
        sections[prevIdx].CompletelyBlock(false);
        sections[newIdx].CompletelyBlock(true);
    }

    private void CheckCurrentPage(int pageIdx, int pageCount)
    {
        button_prev.CompletelyBlock(pageIdx == 0);
        button_next.CompletelyBlock(pageIdx >= pageCount - 1);
    }
}
