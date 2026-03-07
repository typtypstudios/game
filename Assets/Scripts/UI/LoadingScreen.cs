using System.Collections;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 1.5f;
    private CanvasGroup canvasGroup;
    
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;
        GameUIConfigurator.OnUIConfigurated += StartFade;
    }

    private void OnDestroy()
    {
        GameUIConfigurator.OnUIConfigurated -= StartFade;
    }

    private void StartFade() => StartCoroutine(FadeCoroutine());

    IEnumerator FadeCoroutine()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
