using UnityEngine;

public class ReceivedSpellPanel : ACardInfoPanel
{
    [SerializeField] private GameObject sealCross;
    private bool nextIsSealed = false;

    protected override void Awake()
    {
        base.Awake();
        sealCross.SetActive(false);
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
        StartCoroutine(ShowCardCoroutine(cardDef.Image));
    }

    protected override void OnImageSet() 
    {
        sealCross.SetActive(nextIsSealed);
        if (nextIsSealed) nextIsSealed = false;
    }

    protected override void OnCoroutineEnded() { }
}
