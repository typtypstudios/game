using System;
using System.Collections.Generic;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    [SerializeField] private NavigationEntry[] entries;
    private readonly Dictionary<Screens, Canvas> screenDictionary = new();
    private readonly Stack<Screens> screenStack = new();
    private Screens currentScreen;

    private void Awake()
    {
        int enabledCount = 0;
        foreach(var entry in entries)
        {
            screenDictionary[entry.screen] = entry.canvas;
            if (entry.canvas.enabled)
            {
                enabledCount++;
                currentScreen = entry.screen;
            }
        }
        if (enabledCount != 1) Debug.LogError("Error: solo un canvas debe estar activo al inicio.");
    }

    public void GoTo(Screens screen)
    {
        screenStack.Push(currentScreen);
        NavigateToScreen(screen);
    }

    public void GoBack()
    {
        if (screenStack.Count == 0) return;
        NavigateToScreen(screenStack.Pop());
    }

    private void NavigateToScreen(Screens screen)
    {
        foreach(var ctxReceiver in screenDictionary[screen].GetComponentsInChildren<INavigationCtxReceiver>(true))
            ctxReceiver.ReceiveContext(currentScreen);
        screenDictionary[currentScreen].enabled = false;
        currentScreen = screen;
        screenDictionary[currentScreen].enabled = true;
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
    CultSelection
}
