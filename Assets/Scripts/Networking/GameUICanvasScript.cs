using System.Collections;
using TMPro;
using UnityEngine;

public class GameUICanvasScript : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject exitButton;

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

    private void Awake()
    {
        panel.SetActive(false);
        exitButton.SetActive(false);

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

    public void ShowEndMatch(bool isWinner)
    {
        panel.SetActive(true);
        exitButton.SetActive(false);

        resultText.text = isWinner ? "VICTORY" : "DEFEAT";

        WritableText wt = resultText.GetComponent<WritableText>();
        if (wt != null)
        {
            wt.FillColor = isWinner ? Color.cyan : Color.red;
            wt.ResetText();
        }

        exitButton.SetActive(true);
    }
}