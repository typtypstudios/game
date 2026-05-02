using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TypTyp.TextSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RitualTextAnimator : MonoBehaviour
{
    [Min(0.01f)][SerializeField] private float transitionTime = 0.2f;
    [Min(0.01f)][SerializeField] private float appearTime = 1.0f;
    [SerializeField] private NetworkTextProvider provider;
    [SerializeField] private VerticalLayoutGroup textsLayoutGroup;
    private RectTransform[] texts;
    private readonly List<Vector2> initPositions = new();
    private CanvasGroup lastTextGroup;

    private void Start()
    {
        TMP_Text lastText = provider.Texts[^1];
        if (!lastText.TryGetComponent(out lastTextGroup)) 
            lastTextGroup = lastText.AddComponent<CanvasGroup>();
        texts = provider.Texts.Select(t => t.GetComponent<RectTransform>()).ToArray();
        RegisterInitPos();
        provider.OnNextText += PerformAnimation;
    }

    private void RegisterInitPos()
    {
        Canvas.ForceUpdateCanvases();
        for (int i = 0; i < texts.Length; i++)
            initPositions.Add(texts[i].anchoredPosition);
    }

    private void OnDestroy() => provider.OnNextText -= PerformAnimation;

    private void PerformAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(TransitionCoroutine());
        StartCoroutine(LastTextAppearCoroutine());
    }

    IEnumerator TransitionCoroutine()
    {
        textsLayoutGroup.enabled = false;
        for(int i = 0; i < provider.Texts.Length - 1; i++)
            texts[i].anchoredPosition = initPositions[i + 1];
        float speed = Vector2.Distance(texts[0].anchoredPosition, initPositions[0]) / transitionTime;
        while (Vector2.Distance(texts[0].anchoredPosition, initPositions[0]) > 0)
        {
            for (int i = 0; i < texts.Length - 1; i++)
            {
                texts[i].anchoredPosition = Vector3.MoveTowards(texts[i].anchoredPosition, 
                    initPositions[i], speed * Time.deltaTime);
            }
            yield return null;
        }
        textsLayoutGroup.enabled = true;
    }

    IEnumerator LastTextAppearCoroutine()
    {
        lastTextGroup.alpha = 0f;
        float speed = 1 / appearTime;
        while(lastTextGroup.alpha < 1.0)
        {
            lastTextGroup.alpha += speed * Time.deltaTime;
            yield return null;
        }
    }
}
