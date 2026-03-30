using UnityEngine;

public class CastedSpellPanel : ACardInfoPanel
{
    [SerializeField] private GameObject discountAppliedImage;
    [SerializeField] private GameObject sealedImage;
    private CastedSpellInfoType nextType;

    protected override void Awake()
    {
        base.Awake();
        discountAppliedImage.SetActive(false);
        sealedImage.SetActive(false);
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
        StartCoroutine(ShowCardCoroutine(card.Image));
    }

    protected override void OnImageSet() 
    {
        discountAppliedImage.SetActive(nextType == CastedSpellInfoType.Discount);
        sealedImage.SetActive(nextType == CastedSpellInfoType.Seal);
    }

    protected override void OnCoroutineEnded() { }
}

public enum CastedSpellInfoType
{
    Discount,
    Seal
}