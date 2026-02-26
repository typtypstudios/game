using System.Collections;
using System.Collections.Generic;
using TypTyp.TextSystem;
using Unity.Netcode;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private HashSet<ulong> sceneReadyClients = new();

    private class ClientSetupState
    {
        public bool playerConfigured;
        public bool uiConfigured;
    }

    private Dictionary<ulong, ClientSetupState> setupStates = new();
    private HashSet<ulong> fullyConfiguredClients = new();

    private double matchStartTime;
    private bool matchStarting = false;

    //De momento una lista de playerIds server side
    Dictionary<int, Player> playersById;
    public Player GetPlayerById(int id) => playersById.GetValueOrDefault(id);

    //Cambiar esto mas adelante para mas jugadores
    int MaxPlayers => 2;

    public override void OnNetworkSpawn()
    {
        Debug.Log($"MatchManager Spawned | IsServer:{IsServer} | LocalClientId:{NetworkManager.Singleton.LocalClientId}");

        if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        playersById = new();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void OnPlayerReadyRpc(RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong clientId = rpcParams.Receive.SenderClientId;

        if (sceneReadyClients.Contains(clientId))
            return;

        sceneReadyClients.Add(clientId);

        if (sceneReadyClients.Count == MaxPlayers && !matchStarting)
        {
            matchStarting = true;

            setupStates.Clear();
            fullyConfiguredClients.Clear();

            SetupPlayers();
        }
    }

    // Se ejecuta todavía en el server
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

        TryFinalizeClient(clientId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void NotifyUIConfiguredServerRpc(RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!setupStates.ContainsKey(clientId))
            setupStates[clientId] = new ClientSetupState();

        setupStates[clientId].uiConfigured = true;

        TryFinalizeClient(clientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ConfigureUIRpc()
    {
        FindFirstObjectByType<GameUIConfigurator>().ConfigureUI();

        NotifyUIConfiguredServerRpc();
    }



    private void TryFinalizeClient(ulong clientId)
    {
        var state = setupStates[clientId];

        if (state.playerConfigured && state.uiConfigured)
        {
            if (fullyConfiguredClients.Contains(clientId))
                return;

            fullyConfiguredClients.Add(clientId);

            if (fullyConfiguredClients.Count == MaxPlayers)
            {
                PrepareMatchState();    // El server prepara el texto
                StartSynchronizedMatch();
            }
        }
    }

    private void PrepareMatchState()
    {
        // Iniciar la preparación de texto
        var textProvider = FindFirstObjectByType<NetworkTextProvider>();
        textProvider.PrepareTexts();
    }

    private void StartSynchronizedMatch()
    {
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

        while (NetworkManager.Singleton.ServerTime.Time < startTime)
        {
            double remaining = startTime - NetworkManager.Singleton.ServerTime.Time;

            int currentSecond = Mathf.CeilToInt((float)remaining);

            if (currentSecond != lastSecond)
            {
                lastSecond = currentSecond;
                Debug.Log($"CLIENT {NetworkManager.Singleton.LocalClientId} COUNTDOWN: {currentSecond}");
            }

            yield return null;
        }

        Debug.Log($"CLIENT {NetworkManager.Singleton.LocalClientId} COUNTDOWN: GO!");
        BeginMatch();
    }

    /// <summary>
    /// Comienza la partida
    /// </summary>
    private void BeginMatch()
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

        // Comenzar el gameplay
        var player = NetworkManager.Singleton.LocalClient.PlayerObject;
        var textProvider = player.GetComponentInChildren<NetworkTextProvider>();
        textProvider.InitializeTexts();

    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        Debug.Log($"SERVER: Client connected -> {clientId}");

        var player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        Debug.Log($"SERVER: Player spawned for -> {clientId}");
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;

        base.OnDestroy();
    }

}