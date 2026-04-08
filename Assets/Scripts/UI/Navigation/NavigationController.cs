using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

public class NavigationController : MonoBehaviour
{
    [Min(0)][field: SerializeField] public float TransitionTime { get; private set; } = 2;
    [SerializeField] private Material transitionMat;
    [SerializeField] private RenderTexture transitionTexture;
    [SerializeField] private NavigationEntry[] entries;
    private readonly Dictionary<Screens, Canvas> screenDictionary = new();
    private readonly Stack<Screens> screenStack = new();
    private Screens currentScreen;
    private Camera uiCam;
    private bool blocked = false;
    private float Dissolve
    {
        get { return transitionMat.GetFloat("_Dissolve"); }
        set { transitionMat.SetFloat("_Dissolve", Mathf.Clamp01(value)); }
    }

    private void Awake()
    {
        uiCam = GameObject.FindGameObjectWithTag("UICam").GetComponent<Camera>();
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
        InitRT();
        Dissolve = 1;
        StartCoroutine(TransitionCoroutine(initCanvas, initCanvas));
    }

    private void InitRT()
    {
        transitionTexture.Release();
        transitionTexture.width = Screen.width;
        transitionTexture.height = Screen.height;
        transitionTexture.Create();
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
        if (blocked) return;
        if (screenStack.Count == 0) return;
        NavigateToScreen(screenStack.Pop(), true);
    }

    private void NavigateToScreen(Screens screen, bool isGoingBack)
    {
        Canvas originCanvas = screenDictionary[currentScreen];
        Canvas destinationCanvas = screenDictionary[screen];
        INavigationCtxReceiver[] receivers = 
            destinationCanvas.GetComponentsInChildren<INavigationCtxReceiver>(true);
        foreach (var receiver in receivers)
            receiver.ReceiveContext(currentScreen, isGoingBack);
        currentScreen = screen;
        StartCoroutine(TransitionCoroutine(originCanvas, destinationCanvas));
    }

    private IEnumerator TransitionCoroutine(Canvas origin, Canvas dest)
    {
        blocked = true;
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
        uiCam.cullingMask |= (1 << LayerMask.NameToLayer("UI"));
        origin.GetComponent<CanvasGroup>().blocksRaycasts = false;
        float speed = 2 / TransitionTime;
        float dissolveValue = Dissolve; //Para no hacer gets constantes
        while (dissolveValue < 1)
        {
            dissolveValue += speed * Time.deltaTime;
            Dissolve = dissolveValue;
            yield return null;
        }
        origin.enabled = false;
        dest.enabled = true;
        dissolveValue = 1;
        while (dissolveValue > 0)
        {
            dissolveValue -= speed * Time.deltaTime;
            Dissolve = dissolveValue;
            yield return null;
        }
        dest.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Camera.main.cullingMask |= (1 << LayerMask.NameToLayer("UI"));
        uiCam.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
        blocked = false;
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
