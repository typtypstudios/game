using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private int nextSceneIdx = 0;

    public void LoadScene(int sceneIndex, bool waitForTransition)
    {
        nextSceneIdx = sceneIndex;
        if (!waitForTransition) LoadNextScene();
        else CanvasTransitionManager.OnTransitionFinished += LoadNextScene;
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneIdx);
        CanvasTransitionManager.OnTransitionFinished -= LoadNextScene;
    }
}