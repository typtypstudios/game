using UnityEngine;

public class ReceivedSpellPanel : ACardInfoPanel
{
    [SerializeField] private ScratchAnimation scratchAnim;
    private bool nextIsSealed = false;

    protected override void Awake()
    {
        base.Awake();
        scratchAnim.SetScratchAmount(0);
        DeckController.OnAnyCardPlayedEvent += ManageCardApplied;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DeckController.OnAnyCardPlayedEvent -= ManageCardApplied;
    }

    protected override void PerformSubscriptions()
    {
        Player.Enemy.SpellCaster.OnSpellSealed += (_, _) => nextIsSealed = true;
    }

    private void ManageCardApplied(CardEventArgs args)
    {
        if (args.PlayerId == Player.User.OwnerClientId) return;
        var cardDef = CardRegister.Instance.GetById(args.CardId);
        StopAllCoroutines();
        ShowCard(cardDef);
    }

    protected override void OnImageSet() 
    {
        if (nextIsSealed)
        {
            scratchAnim.ScratchAndRemove(showTime, Mathf.Max(fadeTime - scratchAnim.AnimTime, 0));
            nextIsSealed = false;
        }
    }
}
