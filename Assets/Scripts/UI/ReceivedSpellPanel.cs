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
        SpellCaster.OnCardApplied += ManageCardApplied;
        showTimer = new(showTime);
    }

    private void OnDestroy()
    {
        SpellCaster.OnCardApplied -= ManageCardApplied;
    }

    private void ManageCardApplied(ulong casterId, CardDefinition card)
    {
        if (casterId == Player.User.OwnerClientId) return;
        StopAllCoroutines();
        StartCoroutine(ShowCardCoroutine(card.CardImage));
    }

    IEnumerator ShowCardCoroutine(Sprite cardImage)
    {
        while(canvasGroup.alpha > 0) //Por si acaso había alguna carta lanzada ya desplegada
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
