using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class NavigationController : MonoBehaviour
{
    [SerializeField] private NavigationEntry[] entries;
    private CanvasTransitionManager transitionManager;
    private readonly Dictionary<Screens, Canvas> screenDictionary = new();
    private readonly Stack<Screens> screenStack = new();
    private Screens currentScreen;
    private bool blocked = false;

    private void Awake()
    {
        if(!TryGetComponent(out transitionManager))
            Debug.LogError("Error: no hay transition manager asociado al gameObject.");
        int enabledCount = 0;
        Canvas initCanvas = null;
        foreach(var entry in entries)
        {
            screenDictionary[entry.screen] = entry.canvas;
            if (entry.canvas.enabled)
            {
                enabledCount++;
                currentScreen = entry.screen;
                initCanvas = entry.canvas;
            }
            if (!entry.canvas.TryGetComponent(out CanvasGroup canvasGroup))
                canvasGroup = entry.canvas.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
        }
        if (enabledCount != 1) Debug.LogError("Error: solo un canvas debe estar activo al inicio.");
        transitionManager.SubscribeOnStarted(this, () => blocked = true);
        transitionManager.SubscribeOnEnded(this, () => blocked = false);
        transitionManager.PerformTransition(initCanvas, initCanvas, this);
    }

    public void GoTo(Screens screen)
    {
        if (blocked) return;
        if(screen == Screens.GoBack)
        {
            GoBack();
            return;
        }
        screenStack.Push(currentScreen);
        NavigateToScreen(screen, false);
    }

    public void GoBack()
    {
        if (screenStack.Count == 0 || blocked) return;
        NavigateToScreen(screenStack.Pop(), true);
    }

    private void NavigateToScreen(Screens screen, bool isGoingBack)
    {
        Canvas originCanvas = screenDictionary[currentScreen];

        INavigationLeaveReceiver[] leaveReceivers = 
            originCanvas.GetComponentsInChildren<INavigationLeaveReceiver>(true);
        foreach (var receiver in leaveReceivers)
            receiver.OnLeave();

        Canvas destinationCanvas = screenDictionary[screen];
        INavigationCtxReceiver[] receivers = 
            destinationCanvas.GetComponentsInChildren<INavigationCtxReceiver>(true);
        foreach (var receiver in receivers)
            receiver.ReceiveContext(currentScreen, isGoingBack);
        currentScreen = screen;
        blocked = true;
        transitionManager.PerformTransition(originCanvas, destinationCanvas, this);
    }
}

[Serializable]
public class NavigationEntry
{
    public Canvas canvas;
    public Screens screen;
}

public enum Screens
{
    MainMenu,
    Settings,
    Profile,
    InitialTip,
    DeckBuilder,
    Grimoire,
    Credits,
    CultSelection,
    GoBack,
    Loading
}
