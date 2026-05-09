using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerFeedbackUI : MonoBehaviour
{
    [SerializeField] private InputActionReference escActionReference;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private float showDuration = 2f;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        feedbackText.text = "";
        feedbackText.gameObject.SetActive(false);
        MatchManager.OnMatchStarted += Subscribe;
        MatchManager.OnMatchEnded += UnSubscribe;
    }

    private void Subscribe()
    {
        escActionReference.action.performed += ShowEscWarning;
    }

    private void UnSubscribe()
    {
        escActionReference.action.performed -= ShowEscWarning;
    }

    private void OnDestroy()
    {
        MatchManager.OnMatchStarted -= Subscribe;
        MatchManager.OnMatchEnded -= UnSubscribe;
        escActionReference.action.performed -= ShowEscWarning;
    }

    public void ShowSilencedWarning()
    {
        ShowMessage("You are silenced!");
    }

    public void ShowBackspaceWarning()
    {
        ShowMessage("No backspace allowed!");
    }

    public void ShowNotEnoughInkWarning()
    {
        ShowMessage("Not enough ink!");
    }

    private void ShowEscWarning(InputAction.CallbackContext ctx)
    {
        ShowMessage("Pausing mid-ritual?\nPathetic.");
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