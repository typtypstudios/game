using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class StartGameCanvas : MonoBehaviour
{
    [Header("Countdown UI")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private RectTransform leftImage;
    [SerializeField] private RectTransform rightImage;

    [Header("Player Info UI")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text enemyNameText;

    [Header("Animation Settings")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private Vector2 leftImageOffscreenPos = new Vector2(-1500, 0);
    [SerializeField] private Vector2 rightImageOffscreenPos = new Vector2(1500, 0);
    [SerializeField] private Vector2 leftImageCenterPos = new Vector2(-300, 0);
    [SerializeField] private Vector2 rightImageCenterPos = new Vector2(300, 0);

    private Coroutine slideCoroutine;

    public event Action<int> OnCountdownTick;
    public event Action OnCountdownGo;

    private void Awake()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        if (leftImage != null) leftImage.anchoredPosition = leftImageOffscreenPos;
        if (rightImage != null) rightImage.anchoredPosition = rightImageOffscreenPos;
    }

    public void SetCountdownActive(bool isActive)
    {
        countdownText.gameObject.SetActive(isActive);
    }

    public void UpdateCountdownText(string text)
    {
        countdownText.text = text;
    }

    public void NotifyCountdownTick(int second)
    {
        OnCountdownTick?.Invoke(second);
    }

    public void NotifyCountdownGo()
    {
        OnCountdownGo?.Invoke();
    }

    public void AnimateImagesIn()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideImages(leftImageOffscreenPos, leftImageCenterPos, rightImageOffscreenPos, rightImageCenterPos));
    }

    public void AnimateImagesOut()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideImages(leftImageCenterPos, leftImageOffscreenPos, rightImageCenterPos, rightImageOffscreenPos));
    }

    private IEnumerator SlideImages(Vector2 leftStart, Vector2 leftEnd, Vector2 rightStart, Vector2 rightEnd)
    {
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            float t = elapsedTime / slideDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            leftImage.anchoredPosition = Vector2.Lerp(leftStart, leftEnd, smoothT);
            rightImage.anchoredPosition = Vector2.Lerp(rightStart, rightEnd, smoothT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        leftImage.anchoredPosition = leftEnd;
        rightImage.anchoredPosition = rightEnd;
    }
    public void ConfigureUsernames(string player, string enemy)
    {
        if (playerNameText != null)
            playerNameText.text = player;

        if (enemyNameText != null)
            enemyNameText.text = enemy;
    }
    private string GetReasonText(bool isWinner, MatchEndReason reason)
    {
        switch (reason)
        {
            case MatchEndReason.RitualCompleted:
                return isWinner
                    ? "You completed the ritual!"
                    : "Your opponent completed the ritual";

            case MatchEndReason.CorruptionOverflow:
                return isWinner
                    ? "Your opponent succumbed to corruption"
                    : "You succumbed to corruption";

            case MatchEndReason.Disconnection:
                return isWinner
                    ? "Your opponent disconnected"
                    : "You lost connection";

            default:
                return string.Empty;
        }
    }
}