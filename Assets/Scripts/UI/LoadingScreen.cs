using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 1.5f;
    [SerializeField] private Button returnButton;
    private LobbyManager lobbyManager;
    private CanvasGroup canvasGroup;
    private bool isReturning;
    private bool lobbyLostSubscribed;

    void Awake()
    {
        GetComponent<Canvas>().enabled = true;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;
        GameUIConfigurator.OnUIConfigurated += StartFade;
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

        if (lobbyManager != null)
        {
            bool shouldShow = lobbyManager.CanCancel;
            if (returnButton.gameObject.activeSelf != shouldShow)
                returnButton.gameObject.SetActive(shouldShow);
        }
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
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

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
            SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        GameUIConfigurator.OnUIConfigurated -= StartFade;

        if (lobbyLostSubscribed && lobbyManager != null)
        {
            lobbyManager.OnLobbyLost -= OnLobbyLost;
        }
    }

    private void StartFade() => StartCoroutine(FadeCoroutine());

    IEnumerator FadeCoroutine()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
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
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (lobbyManager != null)
        {
            try
            {
                bool cancelled = await lobbyManager.CancelSearchAndLeave();
                if (!cancelled)
                {
                    isReturning = false;
                    if (returnButton != null) returnButton.interactable = true;
                    if (canvasGroup != null)
                    {
                        canvasGroup.interactable = true;
                        canvasGroup.blocksRaycasts = true;
                    }
                    return;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error cancelling search: " + e.Message);
            }
        }

        Debug.Log("Saliendo correctamente de la busqueda de partida");
        SceneManager.LoadScene("MainMenu");
    }
}