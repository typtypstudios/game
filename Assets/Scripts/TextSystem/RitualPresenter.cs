using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RitualPresenter : MonoBehaviour
{
    [SerializeField] private RitualManager ritualManager;
    [SerializeField] private TMP_Text[] texts;
    [SerializeField, Min(0.01f)] private float transitionTime = 0.2f;
    [SerializeField, Min(0.01f)] private float appearTime = 1.0f;
    [SerializeField] private VerticalLayoutGroup textsLayoutGroup;

    private RectTransform[] textRects;
    private readonly List<Vector2> initPositions = new();
    private CanvasGroup lastTextGroup;
    private bool initialized;

    void Awake()
    {
        if (ritualManager == null)
            ritualManager = GetComponentInChildren<RitualManager>(true);

        ClearTexts();
    }

    void OnEnable()
    {
        if (ritualManager == null) return;

        ritualManager.OnTextChanged += HandleTextChanged;
        ritualManager.OnLineCompleted += HandleLineCompleted;
    }

    void Start()
    {
        InitializeLayout();
    }

    void OnDisable()
    {
        if (ritualManager == null) return;

        ritualManager.OnTextChanged -= HandleTextChanged;
        ritualManager.OnLineCompleted -= HandleLineCompleted;
        StopAllCoroutines();
    }

    private void HandleTextChanged(string text)
    {
        RefreshQueue();
    }

    private void HandleLineCompleted(int completedLines)
    {
        PerformAnimation();
    }

    private void RefreshQueue()
    {
        if (ritualManager == null || texts == null || !ritualManager.HasStarted)
            return;

        for (int i = 1; i < texts.Length; i++)
        {
            if (texts[i] == null) continue;
            texts[i].text = ritualManager.GetText(ritualManager.CurrentLineIndex + i);
        }
    }

    private void ClearTexts()
    {
        if (texts == null) return;

        foreach (TMP_Text text in texts)
        {
            if (text != null)
                text.text = string.Empty;
        }
    }

    private void InitializeLayout()
    {
        if (initialized || texts == null || texts.Length == 0)
            return;

        TMP_Text lastText = texts[^1];
        if (lastText != null && !lastText.TryGetComponent(out lastTextGroup))
            lastTextGroup = lastText.gameObject.AddComponent<CanvasGroup>();

        textRects = texts
            .Where(text => text != null)
            .Select(text => text.GetComponent<RectTransform>())
            .Where(rect => rect != null)
            .ToArray();

        Canvas.ForceUpdateCanvases();
        initPositions.Clear();
        for (int i = 0; i < textRects.Length; i++)
            initPositions.Add(textRects[i].anchoredPosition);

        initialized = true;
    }

    private void PerformAnimation()
    {
        InitializeLayout();
        if (!initialized) return;

        StopAllCoroutines();
        StartCoroutine(TransitionCoroutine());
        StartCoroutine(LastTextAppearCoroutine());
    }

    private IEnumerator TransitionCoroutine()
    {
        if (textsLayoutGroup == null || textRects.Length < 2)
            yield break;

        textsLayoutGroup.enabled = false;
        for (int i = 0; i < textRects.Length - 1; i++)
            textRects[i].anchoredPosition = initPositions[i + 1];

        float speed = Vector2.Distance(textRects[0].anchoredPosition, initPositions[0]) / transitionTime;
        while (Vector2.Distance(textRects[0].anchoredPosition, initPositions[0]) > 0)
        {
            for (int i = 0; i < textRects.Length - 1; i++)
            {
                textRects[i].anchoredPosition = Vector3.MoveTowards(
                    textRects[i].anchoredPosition,
                    initPositions[i],
                    speed * Time.deltaTime);
            }

            yield return null;
        }

        textsLayoutGroup.enabled = true;
    }

    private IEnumerator LastTextAppearCoroutine()
    {
        if (lastTextGroup == null)
            yield break;

        lastTextGroup.alpha = 0f;
        float speed = 1 / appearTime;
        while (lastTextGroup.alpha < 1.0f)
        {
            lastTextGroup.alpha += speed * Time.deltaTime;
            yield return null;
        }
    }
}
