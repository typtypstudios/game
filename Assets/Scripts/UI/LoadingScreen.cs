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
    private bool transitioningToGame;
    private bool lobbyEventsSubscribed;
    private bool matchHasOccurred;

    private readonly Dictionary<LoadingMessageType, string> messageDictionary = new();

    void Awake()
    {
        MatchManager.OnClientReadyForCountown += GoToGame;
        MatchManager.OnMatchStarted += MarkMatchStarted;

        foreach (var msg in messages)
        {
            messageDictionary.Add(msg.type, msg.message);
        }
    }

    private void Start()
    {
        SetMessage(LoadingMessageType.Default);
        TryFindAndSubscribeLobbyManager();
    }

    void Update()
    {
        if (returnButton == null || isReturning) return;

        if (matchHasOccurred)
        {
            if (returnButton.gameObject.activeSelf)
                ToggleReturnButton(false);
            return;
        }

        if (lobbyManager == null)
        {
            TryFindAndSubscribeLobbyManager();
        }

        if (lobbyManager != null)
        {
            bool shouldShow = lobbyManager.CanCancel;
            if (returnButton.gameObject.activeSelf != shouldShow)
                ToggleReturnButton(shouldShow);
        }
    }

    private void TryFindAndSubscribeLobbyManager()
    {
        if (lobbyEventsSubscribed) return;

        lobbyManager = FindFirstObjectByType<LobbyManager>();
        if (lobbyManager != null)
        {
            lobbyManager.OnLobbyLost += OnConnectionLost;
            lobbyManager.OnNetworkDisconnected += OnConnectionLost;
            lobbyEventsSubscribed = true;
        }
    }

    private void UnsubscribeLobbyEvents()
    {
        if (lobbyEventsSubscribed && lobbyManager != null)
        {
            lobbyManager.OnLobbyLost -= OnConnectionLost;
            lobbyManager.OnNetworkDisconnected -= OnConnectionLost;
            lobbyEventsSubscribed = false;
        }
    }

    private void MarkMatchStarted()
    {
        matchHasOccurred = true;
        if (returnButton != null && returnButton.gameObject.activeSelf)
            ToggleReturnButton(false);
    }

    private void GoToGame()
    {
        transitioningToGame = true;
        UnsubscribeLobbyEvents();
        NavigationController controller = FindFirstObjectByType<NavigationController>();
        if (!controller)
        {
            Debug.LogError("Error, no se detecta el controlador de navegación");
            GetComponent<Canvas>().enabled = false;
        }
        else controller.GoTo(Screens.Game, this.gameObject);
    }

    private void OnConnectionLost()
    {
        if (isReturning || transitioningToGame) return;

        Debug.Log("LoadingScreen: Conexión o Lobby perdida. Forzando salida.");
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
        MatchManager.OnClientReadyForCountown -= GoToGame;
        MatchManager.OnMatchStarted -= MarkMatchStarted;

        UnsubscribeLobbyEvents();
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
        if (isReturning || matchHasOccurred) return;

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