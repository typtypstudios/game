using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class ACardInfoPanel : MonoBehaviour
{
    [Min(0.01f)][SerializeField] private float fadeTime = 0.5f;
    [Min(0)][SerializeField] private float showTime;
    private CanvasGroup canvasGroup;
    private Image image;
    private WaitForSeconds showTimer;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
        showTimer = new(showTime);
        GameUIConfigurator.OnUIConfigurated += PerformSubscriptions;
    }

    protected virtual void OnDestroy() => GameUIConfigurator.OnUIConfigurated -= PerformSubscriptions;

    protected abstract void PerformSubscriptions();

    protected abstract void OnImageSet();

    protected abstract void OnCoroutineEnded();

    protected IEnumerator ShowCardCoroutine(Sprite cardImage)
    {
        while (canvasGroup.alpha > 0) //Por si acaso hab�a alguna carta lanzada ya desplegada
        {
            canvasGroup.alpha -= Time.deltaTime / fadeTime;
            yield return null;
        }
        image.sprite = cardImage;
        OnImageSet();
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
        OnCoroutineEnded();
    }
}
