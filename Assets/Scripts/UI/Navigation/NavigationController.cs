using System;
using System.Collections.Generic;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    [SerializeField] private Screens initialScreen = Screens.MainMenu;
    [SerializeField] private NavigationEntry[] entries;
    private CanvasTransitionManager transitionManager;
    private CameraNavigation camNavigation;
    private readonly Dictionary<Screens, NavigationEntry> screenDictionary = new();
    private readonly Stack<Screens> screenStack = new();
    private Screens currentScreen;
    private bool blocked = false;

    private void Awake()
    {
        if (!TryGetComponent(out transitionManager))
            Debug.LogError("Error: no hay transition manager asociado al gameObject.");
        if (!TryGetComponent(out camNavigation))
            Debug.LogError("Error: no hay camera navigation asociado al gameObject.");
        foreach (var entry in entries)
        {
            screenDictionary[entry.screen] = entry;
            if (!entry.canvas.TryGetComponent(out CanvasGroup canvasGroup))
                canvasGroup = entry.canvas.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
        }
        transitionManager.SubscribeOnStarted(this, () => blocked = true);
        transitionManager.SubscribeOnEnded(this, () => blocked = false);
    }

    void Start()
    {
        var initCanvas = screenDictionary[initialScreen].canvas;
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
        Canvas originCanvas = screenDictionary[currentScreen].canvas;

        INavigationLeaveReceiver[] leaveReceivers = 
            originCanvas.GetComponentsInChildren<INavigationLeaveReceiver>(true);
        foreach (var receiver in leaveReceivers)
            receiver.OnLeave();

        Canvas destinationCanvas = screenDictionary[screen].canvas;
        INavigationCtxReceiver[] receivers = 
            destinationCanvas.GetComponentsInChildren<INavigationCtxReceiver>(true);
        foreach (var receiver in receivers)
            receiver.ReceiveContext(currentScreen, isGoingBack);
        currentScreen = screen;
        blocked = true;
        transitionManager.PerformTransition(originCanvas, destinationCanvas, this);
        Transform destination = screenDictionary[screen].cameraDestination;
        if (destination != null) camNavigation.MoveTo(destination);
    }
}

[Serializable]
public class NavigationEntry
{
    public Canvas canvas;
    public Screens screen;
    public Transform cameraDestination;
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
