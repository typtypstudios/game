using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using TypTyp;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public enum MatchState
{
    WaitingPlayers,
    ConfiguringPlayers,
    InGame,
    Finished
}

public enum MatchEndReason : byte
{
    RitualCompleted,
    CorruptionOverflow,
    Disconnection
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
    private Dictionary<ulong, PlayerData> playersData = new();

    private double matchStartTime;
    private MatchState matchState;

    // Eventos
    public static event Action OnMatchStarted;
    public static event Action OnMatchEnded;
    public static event Action OnCountdownStarted;

    // Referencia al canvas de inicio y final de la partida
    private StartGameCanvas startGameCanvas;
    private EndGameCanvas endGameCanvas;


    //De momento una lista de playerIds server side
    Dictionary<int, Player> playersById;
    public Player GetPlayerById(int id) => playersById.GetValueOrDefault(id);
    public int GetPlayerId(Player player) => playersById.FirstOrDefault(kvp => kvp.Value == player).Key;

    //Cambiar esto mas adelante para mas jugadores
    int MaxPlayers => 2;

    private bool lobbyShutdownRequested = false;

    private void Awake()
    {
        startGameCanvas = FindFirstObjectByType<StartGameCanvas>();
        if (!startGameCanvas) Debug.LogError("Error: no se encontró el canvas de comienzo de partida");
        endGameCanvas = FindFirstObjectByType<EndGameCanvas>();
        if (!endGameCanvas) Debug.LogError("Error: no se encontró el canvas de finalización de partida");
    }

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

        if (NetworkManager.Singleton.ConnectedClients.Count >= MaxPlayers)
        {
            _ = FindFirstObjectByType<LobbyManager>().UpdateLobbyState("full", locked: true);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void OnPlayerReadyRpc(PlayerData playerData, RpcParams rpcParams = default)
    {
        if (!IsServer) return;
        Debug.Log($"[MatchManager][Server] Player ready:\n{playerData}");

        ulong clientId = rpcParams.Receive.SenderClientId;

        if (sceneReadyClients.Contains(clientId))
            return;

        sceneReadyClients.Add(clientId);
        playersData.TryAdd(clientId, playerData);

        if (sceneReadyClients.Count == MaxPlayers && matchState == MatchState.WaitingPlayers)
        {
            _ = FindFirstObjectByType<LobbyManager>().UpdateLobbyState("starting");
            matchState = MatchState.ConfiguringPlayers;

            setupStates.Clear();
            fullyConfiguredClients.Clear();

            SetupPlayers();
        }
    }

    // Se ejecuta todavia en el server
    private void SetupPlayers()
    {
        playersById.Clear();
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);

        for (int i = 0; i < players.Length; i++)
        {
            playersById.Add(i, players[i]);
            players[i].ConfigureServerPlayer(playersData[players[i].OwnerClientId]);
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
        matchStartTime = NetworkManager.Singleton.ServerTime.Time + 5.0;

        ulong[] clientIds = playersData.Keys.ToArray();
        ulong client1Id = clientIds[0];
        ulong client2Id = clientIds[1];
        string client1Name = playersData[client1Id].PlayerName.ToString();
        string client2Name = playersData[client2Id].PlayerName.ToString();

        // Enviar los datos de los jugadores
        StartMatchClientRpc(matchStartTime, client1Id, client1Name, client2Id, client2Name);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartMatchClientRpc(double startTime, ulong client1Id, string client1Name, ulong client2Id, string client2Name)
    {
        matchState = MatchState.InGame;
        matchStartTime = startTime;

        LobbyManager lobbyManager = FindFirstObjectByType<LobbyManager>();
        if (lobbyManager != null)
            lobbyManager.StopClientPolling();

        Debug.Log($"CLIENT {NetworkManager.Singleton.LocalClientId}: Received StartMatchClientRpc");

        bool isClient1 = NetworkManager.Singleton.LocalClientId == client1Id;
        string localPlayerName = isClient1 ? client1Name : client2Name;
        string enemyPlayerName = isClient1 ? client2Name : client1Name;
        Player.User.name = localPlayerName;
        Player.Enemy.name = enemyPlayerName;
        startGameCanvas.ConfigureUsernames(localPlayerName, enemyPlayerName);
        startGameCanvas.AnimateImagesIn();

        StartCoroutine(WaitForStart(startTime));
    }

    private IEnumerator WaitForStart(double startTime)
    {
        int lastSecond = -1;

        startGameCanvas.SetCountdownActive(true);
        OnCountdownStarted?.Invoke();

        while (true)
        {
            double remaining = startTime - NetworkManager.Singleton.ServerTime.Time;

            if (remaining <= 0)
                break;

            int currentSecond = Mathf.CeilToInt((float)remaining);

            if (currentSecond != lastSecond)
            {
                lastSecond = currentSecond;
                startGameCanvas.UpdateCountdownText(currentSecond.ToString());
                startGameCanvas.NotifyCountdownTick(currentSecond);
            }

            yield return null;
        }

        startGameCanvas.UpdateCountdownText("GO!");
        startGameCanvas.NotifyCountdownGo();
        startGameCanvas.AnimateImagesOut();

        yield return new WaitForSeconds(0.5f);

        startGameCanvas.SetCountdownActive(false);
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

        SaveState saveState = SaveManager.Instance.GetState();
        saveState.slot.profile.numGames += 1;
        SaveManager.Instance.Save();

        OnMatchStarted?.Invoke();
    }

    /// <summary>
    /// System to end the game
    /// </summary>
    /// <param name="winner"></param>
    public void HandlePlayerVictory(Player winner, MatchEndReason reason)
    {
        if (!IsServer || matchState == MatchState.Finished)
            return;
        matchState = MatchState.Finished;
        Player otherPlayer = winner == Player.User ? Player.Enemy : Player.User;
        otherPlayer.CorruptionManager.AddCorruption(Settings.Instance.MaxCorruption);
        EndMatchClientRpc(winner.OwnerClientId, reason);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void EndMatchClientRpc(ulong winnerClientId, MatchEndReason reason)
    {
        // Muestra el mensaje de victoria.

        matchState = MatchState.Finished;

        OnMatchEnded?.Invoke(); // Player input manager está suscrito y desactiva el input

        bool isWinner = NetworkManager.Singleton.LocalClientId == winnerClientId;
        endGameCanvas.ShowEndMatch(isWinner, reason);
        //SpawnLocalPlayerCopy(Player.User);
        //SpawnLocalPlayerCopy(Player.Enemy);
        // Handshake de finalización
        NotifyEndHandledServerRpc();
    }

    private void SpawnLocalPlayerCopy(Player player)
    {
        GameObject copy = Instantiate(player.gameObject,
            player.transform.position,
            player.transform.rotation);
        Destroy(copy.GetComponent<NetworkObject>());
        player.gameObject.SetActive(false);
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
            RequestLobbyShutdown();
        }
    }

    private async Task ShutdownMatchServer()
    {
        LobbyManager lobbyManager = FindFirstObjectByType<LobbyManager>();
        if (lobbyManager != null)
        {
            await lobbyManager.CloseLobyAndShutdown();
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        // Host side
        if (IsServer)
        {
            if (clientId == NetworkManager.ServerClientId)
            {
                if (matchState == MatchState.InGame)
                {
                    matchState = MatchState.Finished;
                    OnMatchEnded?.Invoke();
                    endGameCanvas.ShowEndMatch(false, MatchEndReason.Disconnection);
                }
                RequestLobbyShutdown();
                return;
            }

            switch (matchState)
            {
                case MatchState.WaitingPlayers:
                case MatchState.ConfiguringPlayers:
                    RequestLobbyShutdown();
                    NetworkManager.Singleton.Shutdown();
                    SceneManager.LoadScene("MainMenu");
                    return;

                case MatchState.InGame:
                    StartCoroutine(ResolveHostDisconnect(clientId));
                    return;

                case MatchState.Finished:
                    RequestLobbyShutdown();
                    return;
            }
            return;
        }

        // Client side
        switch (matchState)
        {
            case MatchState.InGame:
                StartCoroutine(ResolveClientDisconnect());
                return;

            case MatchState.WaitingPlayers:
            case MatchState.ConfiguringPlayers:
                NetworkManager.Singleton.Shutdown();
                SceneManager.LoadScene("MainMenu");
                return;

            case MatchState.Finished:
                NetworkManager.Singleton.Shutdown();
                return;
        }
    }

    private IEnumerator ResolveHostDisconnect(ulong disconnectedClientId)
    {
        if (matchState == MatchState.Finished) yield break;
        matchState = MatchState.Finished;

        using var req = UnityWebRequest.Head("https://clients3.google.com/generate_204");
        req.timeout = 3;
        yield return req.SendWebRequest();

        bool haveInternet = req.result == UnityWebRequest.Result.Success;

        if (haveInternet)
        {
            Player winner = null;
            foreach (var kvp in playersById)
            {
                if (kvp.Value != null && kvp.Value.OwnerClientId != disconnectedClientId)
                {
                    winner = kvp.Value;
                    break;
                }
            }

            if (winner != null)
            {
                EndMatchClientRpc(winner.OwnerClientId, MatchEndReason.Disconnection);
            }

            StartCoroutine(DelayedLobbyShutdown(3f));
        }
        else
        {
            OnMatchEnded?.Invoke();
            endGameCanvas.ShowEndMatch(false, MatchEndReason.Disconnection);

            RequestLobbyShutdown();
            NetworkManager.Singleton.Shutdown();
        }
    }

    private IEnumerator ResolveClientDisconnect()
    {
        matchState = MatchState.Finished;
        OnMatchEnded?.Invoke();

        using var req = UnityWebRequest.Head("https://clients3.google.com/generate_204");
        req.timeout = 3;
        yield return req.SendWebRequest();

        bool haveInternet = req.result == UnityWebRequest.Result.Success;

        NetworkManager.Singleton.Shutdown();

        endGameCanvas.ShowEndMatch(haveInternet, MatchEndReason.Disconnection);
    }

    private IEnumerator DelayedLobbyShutdown(float delay)
    {
        yield return new WaitForSeconds(delay);
        RequestLobbyShutdown();
    }

    private void RequestLobbyShutdown()
    {
        if (lobbyShutdownRequested) return;
        lobbyShutdownRequested = true;
        _ = ShutdownMatchServer();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
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