using UnityEngine;
using UnityEngine.UI;

public class GrimoireInfoDisplayer : InfoDisplayer
{
    private static GrimoireInfoDisplayer highlightedDisplayer;
    private GrimoireInfoPanel infoPanel;

    public void Initialize(GrimoireInfoPanel infoPanel)
    {
        this.infoPanel = infoPanel;
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
