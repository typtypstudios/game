using UnityEngine;

public class CursorDisabler : MonoBehaviour
{
    private void Start()
    {
        MatchManager.OnCountdownStarted += HideCursor;
        MatchManager.OnMatchEnded += ShowCursor;
    }

    private void OnDestroy()
    {
        MatchManager.OnCountdownStarted -= HideCursor;
        MatchManager.OnMatchEnded -= ShowCursor;
        ShowCursor();
    }

    private void HideCursor() => Cursor.lockState = CursorLockMode.Locked;
    private void ShowCursor() => Cursor.lockState = CursorLockMode.None;
}
