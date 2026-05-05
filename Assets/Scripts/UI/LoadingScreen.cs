using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 1.5f;
    [SerializeField] private Button returnButton;
    [SerializeField] private WritableText message;
    [SerializeField] private LoadingMessage[] messages;
    private LobbyManager lobbyManager;
    private bool isReturning;
    private bool lobbyLostSubscribed;
    private readonly Dictionary<LoadingMessageType, string> messageDictionary = new();

    void Awake()
    {
        GameUIConfigurator.OnUIConfigurated += GoToGame;
        foreach (var msg in messages)
        {
            messageDictionary.Add(msg.type, msg.message);
        }
        SetMessage(LoadingMessageType.Default);
    }

    private void Start()
    {
        lobbyManager = FindFirstObjectByType<LobbyManager>();
        TrySubscribeLobbyLost();
    }

    void Update()
    {
        if (returnButton == null || isReturning) return;

        if (lobbyManager == null)
        {
            lobbyManager = FindFirstObjectByType<LobbyManager>();
            TrySubscribeLobbyLost();
        }

        if (lobbyManager != null && !lobbyManager.CanCancel)
        { 
            ToggleReturnButton(false);
        }
    }

    private void GoToGame()
    {
        NavigationController controller = FindFirstObjectByType<NavigationController>();
        if (!controller)
        {
            Debug.LogError("Error, no se detecta el controlador de navegación");
            GetComponent<Canvas>().enabled = false;
        }
        else controller.GoTo(Screens.Game, this.gameObject);
    }

    private void TrySubscribeLobbyLost()
    {
        if (lobbyLostSubscribed || lobbyManager == null) return;
        lobbyManager.OnLobbyLost += OnLobbyLost;
        lobbyLostSubscribed = true;
    }

    private void OnLobbyLost()
    {
        Debug.Log("LoadingScreen: lobby perdido. Forzando salida.");
        ForceReturnToMainMenu();
    }

    private async void ForceReturnToMainMenu()
    {
        if (isReturning) return;
        isReturning = true;

        if (returnButton != null) returnButton.interactable = false;

        if (lobbyManager != null)
        {
            try
            {
                await lobbyManager.CloseLobyAndShutdown();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error during forced shutdown: " + e.Message);
            }
        }

        if (SceneManager.GetActiveScene().name != "MainMenu")
            ReturnToMainMenu();
    }

    private void OnDestroy()
    {
        GameUIConfigurator.OnUIConfigurated -= GoToGame;

        if (lobbyLostSubscribed && lobbyManager != null)
        {
            lobbyManager.OnLobbyLost -= OnLobbyLost;
        }
    }

    public void SetMessage(LoadingMessageType type) 
    {
        if (!messageDictionary.ContainsKey(type)) message.SetText("");
        else message.SetText(messageDictionary[type]);
    }

    public void ToggleReturnButton(bool active)
    {
        returnButton.gameObject.SetActive(active);
    }

    public async void OnReturnButtonClicked()
    {
        if (isReturning) return;

        if (lobbyManager != null && !lobbyManager.CanCancel)
        {
            Debug.Log("Return ignorado: la partida ya está empezando.");
            return;
        }

        isReturning = true;

        if (returnButton != null) returnButton.interactable = false;

        if (lobbyManager != null)
        {
            try
            {
                bool cancelled = await lobbyManager.CancelSearchAndLeave();
                if (!cancelled)
                {
                    isReturning = false;
                    if (returnButton != null) returnButton.interactable = true;
                    return;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error cancelling search: " + e.Message);
            }
        }
        ToggleReturnButton(false);
        SetMessage(LoadingMessageType.ReturningToMenu);
        Invoke(nameof(ReturnToMainMenu), 0.1f);
    }

    private void ReturnToMainMenu() => SceneLoader.Instance.LoadScene(0, false);
}

public enum LoadingMessageType
{
    Default,
    LoadingGameScene,
    ReturningToMenu
}

[Serializable]
public class LoadingMessage
{
    public LoadingMessageType type;
    public string message;
}