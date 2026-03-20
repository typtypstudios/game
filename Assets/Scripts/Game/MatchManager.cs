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

public enum SetupType
{
    Player,
    UI
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
    private bool isShuttingDown;

    [SerializeField] private GameUICanvasScript gameUICanvas;

    Dictionary<ulong, Player> playersById;

    public Player GetPlayerById(ulong id) => playersById.GetValueOrDefault(id);
    public Player GetOpponent(ulong myClientId) => playersById.Values.FirstOrDefault(p => p.OwnerClientId != myClientId);

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

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;
        if (clientId == NetworkManager.ServerClientId) return;

        GameObject playerObj = Instantiate(playerPrefab);
        playerObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        Player playerComponent = playerObj.GetComponent<Player>();
        playersById.Add(clientId, playerComponent);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void OnPlayerReadyRpc(PlayerData playerData, RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong clientId = rpcParams.Receive.SenderClientId;
        Debug.Log($"[MatchManager][Server] Player ready: {clientId}");

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

    private void SetupPlayers()
    {
        int index = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            ulong clientId = client.Key;
            if (playersById.TryGetValue(clientId, out Player player))
            {
                player.ConfigureServerPlayer(playersData[clientId]);
                player.ConfigurePlayerRpc(index);
                index++;
            }
        }
        ConfigureUIRpc();
    }

    [Rpc(SendTo.NotServer)]
    public void ConfigureUIRpc()
    {
        FindFirstObjectByType<GameUIConfigurator>().ConfigureUI();
        NotifyConfiguredServerRpc(SetupType.UI);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void NotifyConfiguredServerRpc(SetupType type, RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!setupStates.ContainsKey(clientId))
            setupStates[clientId] = new ClientSetupState();

        switch (type)
        {
            case SetupType.Player:
                setupStates[clientId].playerConfigured = true;
                break;
            case SetupType.UI:
                setupStates[clientId].uiConfigured = true;
                break;
        }

        CheckEverythingReady(clientId);
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

        StartMatchClientRpc(matchStartTime, client1Id, client1Name, client2Id, client2Name);
    }

    [Rpc(SendTo.NotServer)]
    private void StartMatchClientRpc(double startTime, ulong client1Id, string client1Name, ulong client2Id, string client2Name)
    {
        matchState = MatchState.InGame;
        matchStartTime = startTime;

        bool isClient1 = NetworkManager.Singleton.LocalClientId == client1Id;
        string localPlayerName = isClient1 ? client1Name : client2Name;
        string enemyPlayerName = isClient1 ? client2Name : client1Name;

        gameUICanvas.ConfigureUsernames(localPlayerName, enemyPlayerName);
        gameUICanvas.AnimateImagesIn();

        StartCoroutine(WaitForStart(startTime));
    }

    private IEnumerator WaitForStart(double startTime)
    {
        int lastSecond = -1;
        gameUICanvas.SetCountdownActive(true);

        while (true)
        {
            double remaining = startTime - NetworkManager.Singleton.ServerTime.Time;
            if (remaining <= 0) break;

            int currentSecond = Mathf.CeilToInt((float)remaining);
            if (currentSecond != lastSecond)
            {
                lastSecond = currentSecond;
                gameUICanvas.UpdateCountdownText(currentSecond.ToString());
            }
            yield return null;
        }

        gameUICanvas.UpdateCountdownText("GO!");
        gameUICanvas.AnimateImagesOut();

        yield return new WaitForSeconds(0.5f);

        gameUICanvas.SetCountdownActive(false);
        BeginMatchClient();
    }

    private void BeginMatchClient()
    {
        double serverTime = NetworkManager.Singleton.ServerTime.Time;
        double drift = serverTime - matchStartTime;

        Debug.Log($"MATCH STARTED | Client:{NetworkManager.Singleton.LocalClientId} | Drift:{drift * 1000:F2} ms");
        OnMatchStarted?.Invoke();
    }

    public void HandlePlayerVictory(Player winner)
    {
        if (!IsServer || matchState == MatchState.Finished) return;

        matchState = MatchState.Finished;
        EndMatchClientRpc(winner.OwnerClientId);
    }

    [Rpc(SendTo.NotServer)]
    private void EndMatchClientRpc(ulong winnerClientId)
    {
        matchState = MatchState.Finished;
        OnMatchEnded?.Invoke();

        bool isWinner = NetworkManager.Singleton.LocalClientId == winnerClientId;
        gameUICanvas.ShowEndMatch(isWinner);

        NotifyEndHandledServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void NotifyEndHandledServerRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if (endMatchConfirmedClients.Contains(clientId)) return;

        endMatchConfirmedClients.Add(clientId);

        int activePlayersCount = NetworkManager.Singleton.ConnectedClients.Count - 1;

        if (activePlayersCount <= 0 || endMatchConfirmedClients.Count >= activePlayersCount)
        {
            _ = ShutdownMatchServer();
        }
    }

    private async Task ShutdownMatchServer()
    {
        if (isShuttingDown) return;
        isShuttingDown = true;

        LobbyManager lobbyManager = FindFirstObjectByType<LobbyManager>();
        if (lobbyManager != null)
        {
            await lobbyManager.CloseLobyAndShutdown();
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (IsServer)
        {
            if (clientId != NetworkManager.ServerClientId)
            {
                if (matchState == MatchState.InGame || matchState == MatchState.ConfiguringPlayers)
                {
                    matchState = MatchState.Finished;

                    ulong winnerId = playersData.Keys.FirstOrDefault(id => id != clientId);
                    EndMatchClientRpc(winnerId);

                    playersById.Remove(clientId);
                    playersData.Remove(clientId);
                }
                else
                {
                    _ = ShutdownMatchServer();
                    NetworkManager.Singleton.Shutdown();
                    SceneManager.LoadScene("MainMenu");
                }
            }
        }
        else
        {
            if (matchState != MatchState.Finished)
            {
                matchState = MatchState.Finished;
                OnMatchEnded?.Invoke();
                NetworkManager.Singleton.Shutdown();

                if (gameUICanvas != null)
                {
                    gameUICanvas.ShowEndMatch(false);
                }
                else
                {
                    SceneManager.LoadScene("MainMenu");
                }
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