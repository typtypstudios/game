using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReceivedSpellPanel : MonoBehaviour
{
    [Min(0.01f)][SerializeField] private float fadeTime = 0.5f;
    [Min(0)][SerializeField] private float showTime;
    private CanvasGroup canvasGroup;
    private Image image;
    private WaitForSeconds showTimer;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
        DeckController.OnAnyCardPlayedEvent += ManageCardApplied;
        showTimer = new(showTime);
    }

    private void OnDestroy()
    {
        DeckController.OnAnyCardPlayedEvent -= ManageCardApplied;
    }

    // private void ManageCardApplied(ulong casterId, CardDefinition card)
    private void ManageCardApplied(CardEventArgs args)
    {
        if (args.PlayerId == Player.User.OwnerClientId) return;
        var cardDef = CardRegister.Instance.GetById(args.CardId);
        StopAllCoroutines();
        StartCoroutine(ShowCardCoroutine(cardDef.Image));
    }

    IEnumerator ShowCardCoroutine(Sprite cardImage)
    {
        while(canvasGroup.alpha > 0) //Por si acaso hab�a alguna carta lanzada ya desplegada
        {
            canvasGroup.alpha -= Time.deltaTime / fadeTime;
            yield return null;
        }
        image.sprite = cardImage;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / fadeTime;
            yield return null;
        }
        yield return showTimer;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeTime;
            yield return null;
        }
    }
}
