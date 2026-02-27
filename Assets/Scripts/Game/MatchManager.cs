using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using TypTyp.TextSystem;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MatchState
{
    WaitingPlayers,
    ConfiguringPlayers,
    InGame,
    Finished
}

public class MatchManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject playerPrefab;

    private HashSet<ulong> sceneReadyClients = new();

    private class ClientSetupState
    {
        public bool playerConfigured;
        public bool uiConfigured;
    }

    private Dictionary<ulong, ClientSetupState> setupStates = new();
    private HashSet<ulong> fullyConfiguredClients = new();
    private HashSet<ulong> endMatchConfirmedClients = new();

    private double matchStartTime;
    private MatchState matchState;
    public static event Action OnMatchEnded;


    //De momento una lista de playerIds server side
    Dictionary<int, Player> playersById;
    public Player GetPlayerById(int id) => playersById.GetValueOrDefault(id);
    public int GetPlayerId(Player player) => playersById.FirstOrDefault(kvp => kvp.Value == player).Key;

    //Cambiar esto mas adelante para mas jugadores
    int MaxPlayers => 2;

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            playersById = new();
        }
    }

    /// <summary>
    /// Instantiate the prefab
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        var player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void OnPlayerReadyRpc(RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong clientId = rpcParams.Receive.SenderClientId;

        if (sceneReadyClients.Contains(clientId))
            return;

        sceneReadyClients.Add(clientId);

        if (sceneReadyClients.Count == MaxPlayers && matchState == MatchState.WaitingPlayers)
        {
            _ = FindFirstObjectByType<LobbyManager>().UpdateLobbyState("starting");
            matchState = MatchState.ConfiguringPlayers;

            setupStates.Clear();
            fullyConfiguredClients.Clear();

            SetupPlayers();
        }
    }

    // Se ejecuta todav�a en el server
    private void SetupPlayers()
    {
        playersById.Clear();
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);

        for (int i = 0; i < players.Length; i++)
        {
            playersById.Add(i, players[i]);
            players[i].ConfigurePlayerRpc(i);
        }

        ConfigureUIRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void NotifyPlayerConfiguredServerRpc(RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!setupStates.ContainsKey(clientId))
            setupStates[clientId] = new ClientSetupState();

        setupStates[clientId].playerConfigured = true;

        CheckEverythingReady(clientId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void NotifyUIConfiguredServerRpc(RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!setupStates.ContainsKey(clientId))
            setupStates[clientId] = new ClientSetupState();

        setupStates[clientId].uiConfigured = true;

        CheckEverythingReady(clientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ConfigureUIRpc()
    {
        FindFirstObjectByType<GameUIConfigurator>().ConfigureUI();

        NotifyUIConfiguredServerRpc();
    }

    private void CheckEverythingReady(ulong clientId)
    {
        var state = setupStates[clientId];

        if (state.playerConfigured && state.uiConfigured)
        {
            if (fullyConfiguredClients.Contains(clientId))
                return;

            fullyConfiguredClients.Add(clientId);

            if (fullyConfiguredClients.Count == MaxPlayers)
            {
                StartSynchronizedMatch();
            }
        }
    }

    private void StartSynchronizedMatch()
    {
        matchState = MatchState.InGame;
        Debug.Log("SERVER: StartSynchronizedMatch()");
        matchStartTime = NetworkManager.Singleton.ServerTime.Time + 3.0;
        StartMatchClientRpc(matchStartTime);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartMatchClientRpc(double startTime)
    {
        matchStartTime = startTime;

        Debug.Log($"CLIENT {NetworkManager.Singleton.LocalClientId}: Received StartMatchClientRpc");

        StartCoroutine(WaitForStart(startTime));
    }

    private IEnumerator WaitForStart(double startTime)
    {
        int lastSecond = -1;

        countdownText.gameObject.SetActive(true);

        while (NetworkManager.Singleton.ServerTime.Time < startTime)
        {
            double remaining = startTime - NetworkManager.Singleton.ServerTime.Time;
            int currentSecond = Mathf.CeilToInt((float)remaining);

            if (currentSecond != lastSecond)
            {
                lastSecond = currentSecond;
                countdownText.text = currentSecond.ToString();
            }

            yield return null;
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);

        countdownText.gameObject.SetActive(false);

        BeginMatchClient();
    }

    /// <summary>
    /// Comienza la partida
    /// </summary>
    private void BeginMatchClient()
    {
        double localTime = NetworkManager.Singleton.LocalTime.Time;
        double serverTime = NetworkManager.Singleton.ServerTime.Time;

        double drift = serverTime - matchStartTime;

        Debug.Log(
            $"MATCH STARTED | " +
            $"Client:{NetworkManager.Singleton.LocalClientId} | " +
            $"ServerTime:{serverTime:F4} | " +
            $"Scheduled:{matchStartTime:F4} | " +
            $"Drift:{drift * 1000:F2} ms"
        );

        NetworkTextProvider networkText = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<NetworkTextProvider>();
        if( networkText != null ) networkText.SetTextsActive(true);
    }

    /// <summary>
    /// System to end the game
    /// </summary>
    /// <param name="winner"></param>
    public void HandlePlayerVictory(Player winner)
    {
        if (!IsServer || matchState == MatchState.Finished)
            return;

        matchState = MatchState.Finished;

        EndMatchClientRpc(winner.OwnerClientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void EndMatchClientRpc(ulong winnerClientId)
    {
        // Muestra el mensaje de victoria. El botón todavía no se habilita.
        // El botón de salida se habilita automáticamente al detectar que se ha desconectado del NetworkManager.
        // La verdad he hecho esto porque me ha dado la gana XD. Así me aseguro con los handshakes y eso
        // probablemente haya una mejor manera pero estoy cansado.

        bool isWinner = NetworkManager.Singleton.LocalClientId == winnerClientId;

        DisableGameplay();

        FindFirstObjectByType<EndGamePanel>().Show(isWinner);
        OnMatchEnded?.Invoke();

        NotifyEndHandledServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void NotifyEndHandledServerRpc(RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong clientId = rpcParams.Receive.SenderClientId;

        if (endMatchConfirmedClients.Contains(clientId))
            return;

        endMatchConfirmedClients.Add(clientId);

        if (endMatchConfirmedClients.Count == MaxPlayers)
        {
            _ = ShutdownMatchServer();
        }
    }

    private async Task ShutdownMatchServer()
    {
        if (!IsServer) return;

        var lobbyManager = FindFirstObjectByType<LobbyManager>();
        if (lobbyManager != null)
        {
            await lobbyManager.CloseLobyAndShutdown();
        }

        if (IsHost)
        {
            var panel = FindFirstObjectByType<EndGamePanel>();
            if (panel != null)
            {
                panel.OnNetworkClosed();
            }
        }
    }


    private void DisableGameplay()
    {
        var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (playerObject == null) return;

        var ritual = playerObject.GetComponentInChildren<RitualManager>();
        if (ritual != null)
            ritual.SetActive(false);

        // NOTA: desactivar también la escritura de hechizos
        Debug.LogWarning("Spell deactivation not implemented");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsListening)
            return;

        if (clientId != NetworkManager.Singleton.LocalClientId)
            return;

        var panel = FindFirstObjectByType<EndGamePanel>();
        if (panel != null)
        {
            panel.OnNetworkClosed();
        }

        NetworkManager.Singleton.Shutdown();
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        base.OnDestroy();
    }

}