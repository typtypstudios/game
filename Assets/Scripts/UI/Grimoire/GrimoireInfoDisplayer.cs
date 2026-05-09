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
    
    public void ForceSelect()
    {
        infoPanel.SetInfo(Definition, true);
        ChangeHighlighted();
    }

    public void DisplayInfo()
    {
        infoPanel.SetInfo(Definition);
        ChangeHighlighted();
    }

    private void ChangeHighlighted()
    {
        if (highlightedDisplayer) highlightedDisplayer.Highlight(false);
        highlightedDisplayer = this;
        Highlight(true);
    }

    public override void Highlight(bool highlight)
    {
        base.Highlight(highlight);
        writableButton.CompletelyBlock(highlight);
    }
}
