

public class GrimoireInfoDisplayer : InfoDisplayer
{
    private static GrimoireInfoDisplayer highlightedDisplayer;
    private GrimoireInfoPanel infoPanel;
    private CardDefinition card;
    private StatusEffectDefinition effect;

    private void OnEnable()
    {
        infoPanel = FindFirstObjectByType<GrimoireInfoPanel>();   
    }

    public override void SetCard(CardDefinition card)
    {
        base.SetCard(card);
        this.effect = null;
        this.card = card;
    }

    public override void SetEffect(StatusEffectDefinition effect)
    {
        base.SetEffect(effect);
        this.card = null;
        this.effect = effect;
    }

    public void DisplayInfo()
    {
        if (card) infoPanel.SetInfo(card);
        else infoPanel.SetInfo(effect);
        if (highlightedDisplayer) highlightedDisplayer.Highlight(false);
        highlightedDisplayer = this;
        Highlight(true);
    }
}
