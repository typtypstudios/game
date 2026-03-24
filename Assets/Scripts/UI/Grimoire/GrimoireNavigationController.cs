using UnityEngine;

public class GrimoireNavigationController : MonoBehaviour
{
    [SerializeField] private WritableButton[] sections;
    [SerializeField] private WritableButton button_prev;
    [SerializeField] private WritableButton button_next;
    private GrimoireContentManager contentManager;

    private void Awake()
    {
        contentManager = GetComponentInParent<GrimoireContentManager>();
        contentManager.OnPageChanged += CheckCurrentPage;
        contentManager.OnSectionChanged += CheckCurrentSection;
    }

    private void OnDestroy()
    {
        contentManager.OnPageChanged -= CheckCurrentPage;
        contentManager.OnSectionChanged -= CheckCurrentSection;
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
