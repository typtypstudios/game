using UnityEngine;

public class CardAudioPlayer : MonoBehaviour
{
    private void OnEnable()
    {
        DeckController.OnAnyCardPlayedEvent += HandleCardPlayed;
    }

    private void OnDisable()
    {
        DeckController.OnAnyCardPlayedEvent -= HandleCardPlayed;
    }

    private void HandleCardPlayed(CardEventArgs args)
    {
        var cardDef = CardRegister.Instance.GetById(args.CardId);
        if (cardDef == null || cardDef.CastSound == null) return;
        if (AudioManager.Instance == null) return;

        AudioManager.Instance.PlaySFX(cardDef.CastSound, cardDef.CastVolume);
    }
}