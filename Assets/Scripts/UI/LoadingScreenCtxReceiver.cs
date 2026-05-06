using UnityEngine;
[RequireComponent(typeof(LoadingScreen))]
public class LoadingScreenCtxReceiver : MonoBehaviour, INavigationCtxReceiver
{
    private LoadingScreen loadingScreen;

    private void Awake()
    {
        loadingScreen = GetComponent<LoadingScreen>();
    }

    public void ReceiveContext(Screens prevScreen, bool isGoingBack, GameObject sender = null)
    {
        if (prevScreen == Screens.Results)
        {
            loadingScreen.SetMessage(LoadingMessageType.ReturningToMenu);
            loadingScreen.ToggleReturnButton(false);
        }
        else if (prevScreen == Screens.CultSelection)
            loadingScreen.SetMessage(LoadingMessageType.LoadingGameScene);
        foreach (var wt in GetComponentsInChildren<WritableText>(true))
            wt.ResetText();
    }
}