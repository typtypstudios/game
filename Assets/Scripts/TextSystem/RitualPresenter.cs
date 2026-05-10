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
    [SerializeField, Min(0.01f)] private float groupAppearTime = 1.0f;
    [SerializeField] private VerticalLayoutGroup textsLayoutGroup;
    [SerializeField] private CanvasGroup placeholderGroup;
    [SerializeField] private CanvasGroup textsGroup;
    [SerializeField] private StartGameCanvas startGameCanvas;

    private RectTransform[] textRects;
    private readonly List<Vector2> initPositions = new();
    private CanvasGroup lastTextGroup;
    private bool initialized;
    private Coroutine transitionCoroutine;
    private Coroutine lastTextAppearCoroutine;
    private Coroutine groupAppearCoroutine;

    void Awake()
    {
        if (ritualManager == null)
            ritualManager = GetComponentInChildren<RitualManager>(true);

        if (textsGroup == null)
            textsGroup = GetComponent<CanvasGroup>();

        if (startGameCanvas == null)
            startGameCanvas = FindFirstObjectByType<StartGameCanvas>();

        SetGroupAlpha(textsGroup, 0f);
        SetGroupAlpha(placeholderGroup, 1f);
        ClearTexts();
    }

    void OnEnable()
    {
        if (ritualManager != null)
        {
            ritualManager.OnTextChanged += HandleTextChanged;
            ritualManager.OnLineCompleted += HandleLineCompleted;
        }

        if (startGameCanvas == null)
            startGameCanvas = FindFirstObjectByType<StartGameCanvas>();

        if (startGameCanvas != null)
            startGameCanvas.OnCountdownTick += HandleCountdownTick;
    }

    void Start()
    {
        InitializeLayout();
    }

    void OnDisable()
    {
        if (ritualManager != null)
        {
            ritualManager.OnTextChanged -= HandleTextChanged;
            ritualManager.OnLineCompleted -= HandleLineCompleted;
        }

        if (startGameCanvas != null)
            startGameCanvas.OnCountdownTick -= HandleCountdownTick;

        StopAllCoroutines();
        transitionCoroutine = null;
        lastTextAppearCoroutine = null;
        groupAppearCoroutine = null;
    }

    private void HandleTextChanged(string text)
    {
        RefreshQueue();
    }

    private void HandleLineCompleted(int completedLines)
    {
        PerformAnimation();
    }

    private void HandleCountdownTick(int second)
    {
        if (second != 2 || ritualManager == null)
            return;

        ritualManager.PrepareRitualTexts();
        StartGroupAppearAnimation();
    }

    private void RefreshQueue()
    {
        if (ritualManager == null || texts == null || !ritualManager.HasPreparedTexts)
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

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);
        if (lastTextAppearCoroutine != null)
            StopCoroutine(lastTextAppearCoroutine);

        transitionCoroutine = StartCoroutine(TransitionCoroutine());
        lastTextAppearCoroutine = StartCoroutine(LastTextAppearCoroutine());
    }

    private void StartGroupAppearAnimation()
    {
        if (textsGroup == null)
            return;

        if (groupAppearCoroutine != null)
            StopCoroutine(groupAppearCoroutine);

        groupAppearCoroutine = StartCoroutine(GroupAppearCoroutine());
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
        transitionCoroutine = null;
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

        lastTextGroup.alpha = 1f;
        lastTextAppearCoroutine = null;
    }

    private IEnumerator GroupAppearCoroutine()
    {
        float speed = 1 / groupAppearTime;

        SetGroupAlpha(placeholderGroup, 1f);
        while (placeholderGroup.alpha > 0.0f)
        {
            SetGroupAlpha(placeholderGroup, placeholderGroup.alpha - speed * Time.deltaTime);
            yield return null;
        }
        SetGroupAlpha(placeholderGroup, 0f);

        SetGroupAlpha(textsGroup, 0f);
        while (textsGroup.alpha < 1.0f)
        {
            SetGroupAlpha(textsGroup, textsGroup.alpha + speed * Time.deltaTime);
            yield return null;
        }
        SetGroupAlpha(textsGroup, 1f);

        groupAppearCoroutine = null;
    }

    private void SetGroupAlpha(CanvasGroup group, float alpha)
    {
        if (group != null)
            group.alpha = Mathf.Clamp01(alpha);
    }
}
