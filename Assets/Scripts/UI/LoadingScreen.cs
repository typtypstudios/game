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
    }

    void Update()
    {
        if (returnButton == null || isReturning) return;

        if (lobbyManager == null)
            lobbyManager = FindFirstObjectByType<LobbyManager>();

        if (lobbyManager != null)
        {
            bool shouldShow = lobbyManager.CanCancel;
            if (returnButton.gameObject.activeSelf != shouldShow)
                returnButton.gameObject.SetActive(shouldShow);
        }
    }

    private void OnDestroy()
    {
        GameUIConfigurator.OnUIConfigurated -= StartFade;
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

        // Si no se puede cancelar se ignora
        if (lobbyManager != null && !lobbyManager.CanCancel)
        {
            Debug.Log("Return ignorado: la partida ya está empezando.");
            return;
        }

        isReturning = true;

        // Inhibir todo por si acaso
        if (returnButton != null)
        {
            returnButton.interactable = false;
        }
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        // Aceder al lobbyManager, cerrar todo, esperar, y salir de la escena
        if (lobbyManager != null)
        {
            try
            {
                bool cancelled = await lobbyManager.CancelSearchAndLeave();
                if (!cancelled)
                {
                    // Revertir por si acaso
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