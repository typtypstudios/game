using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NavigationController : MonoBehaviour
{
    [SerializeField] private InputActionReference goBackAction;
    [SerializeField] private bool allowsGoBack = true;
    [SerializeField] private NavigationEntry[] entries;
    [Header("Initial screen transition")]
    [SerializeField] private Screens initialScreen = Screens.MainMenu;
    [SerializeField] private bool startsWithTransition = true;
    [SerializeField] private float initialScreenAppearTimer = 1f;
    [SerializeField] private float initialTransitionTime = 2f;
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
            entry.canvas.enabled = false;
        }
        transitionManager.SubscribeOnStarted(this, () => blocked = true);
        transitionManager.SubscribeOnEnded(this, () => blocked = false);
        goBackAction.action.started += GoBackAction;
        currentScreen = initialScreen;
    }

    private void Start()
    {
        if (startsWithTransition) StartCoroutine(PerformFirstTransition());
        else
        {
            Canvas canvas = screenDictionary[initialScreen].canvas;
            canvas.enabled = true;
            canvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    private void OnDestroy() => goBackAction.action.started -= GoBackAction;

    IEnumerator PerformFirstTransition()
    {
        //transitionManager.SetDissolve(1);
        yield return new WaitForSeconds(initialScreenAppearTimer);
        var initCanvas = screenDictionary[initialScreen].canvas;
        transitionManager.PerformTransition(initCanvas, initCanvas, this, true, initialTransitionTime);
    }

    public void GoTo(Screens screen, GameObject sender)
    {
        if (blocked) return;
        if(screen == Screens.GoBack)
        {
            GoBack();
            return;
        }
        screenStack.Push(currentScreen);
        NavigateToScreen(screen, false, sender);
    }

    private void GoBackAction(InputAction.CallbackContext _) => GoBack();

    public void GoBack()
    {
        if (screenStack.Count == 0 || blocked || !allowsGoBack) return;
        NavigateToScreen(screenStack.Pop(), true);
    }

    private void NavigateToScreen(Screens screen, bool isGoingBack, GameObject sender = null)
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
            receiver.ReceiveContext(currentScreen, isGoingBack, sender);
        currentScreen = screen;
        blocked = true;
        transitionManager.PerformTransition(originCanvas, destinationCanvas, this, true);
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
    Loading,
    Tutorial,
    Game,
    Results,
    Lore
}
