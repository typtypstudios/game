using UnityEngine;

public class CastedSpellPanel : ACardInfoPanel
{
    [SerializeField] private GameObject discountAppliedImage;
    [SerializeField] private ScratchAnimation scratchAnim;
    private CastedSpellInfoType nextType;

    protected override void Awake()
    {
        base.Awake();
        discountAppliedImage.SetActive(false);
        scratchAnim.SetScratchAmount(0);
    }

    protected override void PerformSubscriptions()
    {
        //En este caso los players mueren cuando la partida acaba,
        //y lo mismo hacen los paneles, no hace falta desuscribir
        Player.User.SpellCaster.OnSpellSealed += (card, _) => DisplayInfo(card, CastedSpellInfoType.Seal);
        Player.User.DeckController.OnDiscountApplied += 
            (card) => DisplayInfo(card, CastedSpellInfoType.Discount);
    }

    public void DisplayInfo(CardDefinition card, CastedSpellInfoType type)
    {
        nextType = type;
        ShowCard(card);
    }

    protected override void OnImageSet() 
    {
        discountAppliedImage.SetActive(nextType == CastedSpellInfoType.Discount);
        if (nextType == CastedSpellInfoType.Seal)
            scratchAnim.ScratchAndRemove(showTime, Mathf.Max(fadeTime - scratchAnim.AnimTime, 0));
        else scratchAnim.SetScratchAmount(0);
    }
}

public enum CastedSpellInfoType
{
    Discount,
    Seal
}
