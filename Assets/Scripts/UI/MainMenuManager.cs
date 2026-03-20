using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Por ahora para que funcione lo hago de manera estatica, ya lo cambiare despues
    // Me duele escribir comentarios sin tildes
    public static bool IsDedicatedServer;

    public void PlayAsClient()
    {
        IsDedicatedServer = false;
        SceneManager.LoadScene("MatchScene");
    }

    // Play pero como server
    public void PlayAsServer()
    {
        IsDedicatedServer = true;
        SceneManager.LoadScene("MatchScene");
    }

    public void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
