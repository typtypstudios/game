using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireNavigationController : MonoBehaviour
{
    [SerializeField] private GameObject sectionButtonPrefab;
    [SerializeField] private Transform sectionsTransform;
    [SerializeField] private WritableButton button_prev;
    [SerializeField] private WritableButton button_next;
    private readonly List<SectionButton> sections = new();
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

    public void AddSection(int index, string name, int cultId)
    {
        SectionButton s = Instantiate(sectionButtonPrefab, sectionsTransform).GetComponent<SectionButton>();
        s.GetComponent<Button>().onClick.AddListener(() => contentManager.GoToSection(index));
        sections.Add(s);
        s.Configurate(name, cultId);
        if (cultId != -1)
        {
            CultColoredItem item = s.AddComponent<CultColoredItem>();
            item.FixCult(cultId);
        }
    }

    public void CheckCurrentSection(int newIdx, int prevIdx)
    {
        sections[prevIdx].Hide();
        sections[newIdx].Deploy();
    }

    private void CheckCurrentPage(int pageIdx, int pageCount)
    {
        button_prev.CompletelyBlock(pageIdx == 0);
        button_next.CompletelyBlock(pageIdx >= pageCount - 1);
    }
}
