using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
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
    private Dictionary<ulong, PlayerData> playersData = new();

    private double matchStartTime;
    private MatchState matchState;
    public static event Action OnMatchStarted;
    public static event Action OnMatchEnded;

    // Referencia al canvas de inicio y final de la partida
    [SerializeField] private StartEndCanvas canvasUIScript;


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
        matchStartTime = NetworkManager.Singleton.ServerTime.Time + 3.0;

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

        Debug.Log($"CLIENT {NetworkManager.Singleton.LocalClientId}: Received StartMatchClientRpc");

        bool isClient1 = NetworkManager.Singleton.LocalClientId == client1Id;
        string localPlayerName = isClient1 ? client1Name : client2Name;
        string enemyPlayerName = isClient1 ? client2Name : client1Name;
        Player.User.name = localPlayerName;
        Player.Enemy.name = enemyPlayerName;
        canvasUIScript.ConfigureUsernames(localPlayerName, enemyPlayerName);
        canvasUIScript.AnimateImagesIn();

        StartCoroutine(WaitForStart(startTime));
    }

    private IEnumerator WaitForStart(double startTime)
    {
        int lastSecond = -1;

        canvasUIScript.SetCountdownActive(true);

        while (true)
        {
            double remaining = startTime - NetworkManager.Singleton.ServerTime.Time;

            if (remaining <= 0)
                break;

            int currentSecond = Mathf.CeilToInt((float)remaining);

            if (currentSecond != lastSecond)
            {
                lastSecond = currentSecond;
                canvasUIScript.UpdateCountdownText(currentSecond.ToString());
            }

            yield return null;
        }

        canvasUIScript.UpdateCountdownText("GO!");
        canvasUIScript.AnimateImagesOut();

        yield return new WaitForSeconds(0.5f);

        canvasUIScript.SetCountdownActive(false);
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

        OnMatchStarted?.Invoke();
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
        // Muestra el mensaje de victoria.

        matchState = MatchState.Finished;

        OnMatchEnded?.Invoke(); // Player input manager está suscrito y desactiva el input

        bool isWinner = NetworkManager.Singleton.LocalClientId == winnerClientId;
        canvasUIScript.ShowEndMatch(isWinner);

        // Handshake de finalización
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
        LobbyManager lobbyManager = FindFirstObjectByType<LobbyManager>();
        if (lobbyManager != null)
        {
            await lobbyManager.CloseLobyAndShutdown();
        }
    }

    // Llamado desde el botón
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!IsServer && (clientId == NetworkManager.ServerClientId || clientId == NetworkManager.Singleton.LocalClientId))
        {
            if (matchState == MatchState.ConfiguringPlayers || matchState == MatchState.WaitingPlayers)
            {
                // Salir de la partida
                NetworkManager.Singleton.Shutdown();
                SceneManager.LoadScene("MainMenu");
            }
            else if (matchState == MatchState.InGame)
            {
                // Terminar la partida
                matchState = MatchState.Finished;
                OnMatchEnded?.Invoke(); // Player input manager está suscrito y desactiva el input
                NetworkManager.Singleton.Shutdown();

                canvasUIScript.ShowEndMatch(clientId != Player.User.OwnerClientId);
            }
        }

        // Caso del host sigue vivo
        if (clientId != NetworkManager.ServerClientId)
        {
            if (matchState == MatchState.ConfiguringPlayers || matchState == MatchState.WaitingPlayers)
            {
                // Salir de la partida
                _ = ShutdownMatchServer();
                NetworkManager.Singleton.Shutdown();
                SceneManager.LoadScene("MainMenu");
            }
            else if (matchState == MatchState.InGame)
            {
                // Terminar la partida
                matchState = MatchState.Finished;
                OnMatchEnded?.Invoke(); // Player input manager está suscrito y desactiva el input
                _ = ShutdownMatchServer();
                NetworkManager.Singleton.Shutdown();

                canvasUIScript.ShowEndMatch(clientId != Player.User.OwnerClientId);
            }
        }
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