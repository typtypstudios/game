using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerFeedbackUI : MonoBehaviour
{
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private float showDuration = 2f;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        feedbackText.gameObject.SetActive(false);
    }

    public void ShowSilencedWarning()
    {
        ShowMessage("You are silenced!");
    }

    private void ShowMessage(string message)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(DisplayAndFadeText(message));
    }

    private IEnumerator DisplayAndFadeText(string message)
    {
        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);

        yield return new WaitForSeconds(showDuration);
        feedbackText.gameObject.SetActive(false);
    }
}