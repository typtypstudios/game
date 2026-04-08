using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Play(float timer = 0)
    {
        StartCoroutine(PlayAfterTimer(timer));
    }

    public void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    IEnumerator PlayAfterTimer(float timer)
    {
        yield return new WaitForSeconds(timer);
        SceneManager.LoadScene("MatchScene");
    }
}
