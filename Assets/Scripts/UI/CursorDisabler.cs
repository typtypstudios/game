using UnityEngine;

public class CursorDisabler : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        MatchManager.OnMatchEnded += ShowCursor;
    }

    private void OnDestroy()
    {
        MatchManager.OnMatchEnded -= ShowCursor;
        ShowCursor();
    }

    private void ShowCursor() => Cursor.lockState = CursorLockMode.None;
}
