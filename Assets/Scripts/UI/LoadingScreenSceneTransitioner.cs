using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LoadingScreen))]
public class LoadingScreenSceneTransitioner : MonoBehaviour
{
    private void Start()
    {
        if(SceneManager.GetActiveScene().name.Equals("MatchScene")) //Escena de juego
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        CanvasTransitionManager.OnDissolved -= DestroyOnDissolve; 
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CanvasTransitionManager.OnDissolved += DestroyOnDissolve;
        GetComponent<CanvasTypeFixer>().SetCanvasType(RenderMode.ScreenSpaceCamera);
    }

    private void DestroyOnDissolve() => Destroy(this.gameObject);
}