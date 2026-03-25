

using UnityEngine.UI;

public class GrimoireInfoDisplayer : InfoDisplayer
{
    private static GrimoireInfoDisplayer highlightedDisplayer;
    private GrimoireInfoPanel infoPanel;

    private void OnEnable()
    {
        infoPanel = FindFirstObjectByType<GrimoireInfoPanel>();   
    }

    public void PerformClick() => GetComponent<Button>().onClick?.Invoke();

    public void DisplayInfo()
    {
        infoPanel.SetInfo(Definition);
        if (highlightedDisplayer) highlightedDisplayer.Highlight(false);
        highlightedDisplayer = this;
        Highlight(true);
    }
}
