using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using LobbyPlayer = Unity.Services.Lobbies.Models.Player;

public class LobbyManager : MonoBehaviour
{
    const int MaxPlayers = 3;   // Osea son 2 pero con el server pues 3 lol
    [Tooltip("Intervalo en segundos para enviar el latido de la lobby")]
    [SerializeField] private float heartbeatInterval = 10f;

    Lobby currentLobby;
    bool isMatching;

    // Valores por defecto para pruebas locales
    private string serverIpAddress = "127.0.0.1";
    private ushort serverPort = 7777;

    async void Awake()
    {
        ReadServerArguments();

        await InitializeServices();

        if (MainMenuManager.IsDedicatedServer)
        {
            _ = CreateLobby();
        }
        else
        {
            _ = SearchLobby();
        }
    }

    private void ReadServerArguments()
    {
        // Obtenemos todos los argumentos con los que se inició el programa
        string[] args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-serverIp" && args.Length > i + 1)
            {
                serverIpAddress = args[i + 1];
                Debug.Log($"[Server Args] IP sobreescrita por comandos: {serverIpAddress}");
            }
            else if (args[i] == "-serverPort" && args.Length > i + 1)
            {
                if (ushort.TryParse(args[i + 1], out ushort parsedPort))
                {
                    serverPort = parsedPort;
                    Debug.Log($"[Server Args] Puerto sobreescrito por comandos: {serverPort}");
                }
            }
        }
    }

    async Task InitializeServices()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        }
    }

    public async Task SearchLobby()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        if (isMatching) return;
        isMatching = true;

        try
        {
            for (int i = 0; i < 5; ++i)
            {
                try
                {
                    QueryResponse query = await LobbyService.Instance.QueryLobbiesAsync(
                        new QueryLobbiesOptions
                        {
                            Filters = new List<QueryFilter>
                            {
                                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                                new QueryFilter(QueryFilter.FieldOptions.S1, "waiting", QueryFilter.OpOptions.EQ)
                            }
                        }
                    );

                    // Intentar unirse al primer lobby disponible
                    // Si el servidor nos rechaza, JoinLobby devolverá false, y el bucle continuará (o pasará a la siguiente iteración).
                    foreach (var lobby in query.Results)
                    {
                        if (await JoinLobby(lobby)) return;
                    }

                    // Esperar un par de segundillos antes de volver a intentar buscar lobbies, para evitar spamear el servicio en caso de que no haya lobbies disponibles.
                    await Task.Delay(2000);
                }
                catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    Debug.Log("Rate limited. Waiting before retry...");
                    await Task.Delay(1000);
                }
                catch (Exception e)
                {
                    Debug.Log("Matchmaking failed: " + e);
                    await Task.Delay(1000);
                }
            }

            Debug.Log("No se encontraron servidores disponibles tras varios intentos. Volviendo al menú principal.");
            SceneManager.LoadScene("MainMenu");
        }
        finally
        {
            isMatching = false;
        }
    }

    // El Servidor Dedicado crea la lobby y expone su IP y Puerto
    async Task<bool> CreateLobby()
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { "serverIP", new DataObject(DataObject.VisibilityOptions.Public, serverIpAddress) },
                    { "serverPort", new DataObject(DataObject.VisibilityOptions.Public, serverPort.ToString()) },
                    { "state", new DataObject(DataObject.VisibilityOptions.Public, "waiting", DataObject.IndexOptions.S1) }
                }
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync("DedicatedServer", MaxPlayers, options);

            ConfigureTransport(serverIpAddress, serverPort);

            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;

            // Start as server
            if (!NetworkManager.Singleton.StartServer())
            {
                Debug.LogWarning("Failed to start server");
                await CloseLobyAndShutdown();
                return false;
            }

            InvokeRepeating(nameof(SendHeartbeatWrapper), heartbeatInterval, heartbeatInterval);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning("CreateLobby failed: " + e);
            await CloseLobyAndShutdown();
            return false;
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        int currentPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count;

        if (currentPlayers >= MaxPlayers)
        {
            response.Approved = false;
            response.Reason = "La partida ya esta llena.";
            Debug.Log("[Connection Approval] Conexion rechazada: Servidor lleno.");
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = false;
        response.Pending = false;
    }

    void SendHeartbeatWrapper()
    {
        _ = HeartbeatLobby();
    }

    // El Cliente extrae la IP y el Puerto de la Lobby y se conecta
    async Task<bool> JoinLobby(Lobby lobby)
    {
        try
        {
            Debug.Log($"Intentando unirse a la lobby {lobby.Id}...");

            var playerData = new Dictionary<string, PlayerDataObject>
            {
                { "sessionId", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, AuthenticationService.Instance.PlayerId) }
            };

            currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions { Player = new LobbyPlayer(data: playerData) });

            if (currentLobby == null || !currentLobby.Data.ContainsKey("serverIP") || !currentLobby.Data.ContainsKey("serverPort"))
            {
                Debug.LogWarning("Datos de lobby inválidos o falta IP/Puerto");
                return false;
            }

            string ip = currentLobby.Data["serverIP"].Value;
            ushort port = ushort.Parse(currentLobby.Data["serverPort"].Value);

            ConfigureTransport(ip, port);

            var tcs = new TaskCompletionSource<bool>();

            void OnClientDisconnect(ulong clientId)
            {
                if (clientId == NetworkManager.Singleton.LocalClientId || clientId == 0)
                {
                    Debug.LogWarning("El servidor nos ha rechazado o no hemos podido conectar.");
                    tcs.TrySetResult(false);
                }
            }

            void OnClientConnected(ulong clientId)
            {
                if (clientId == NetworkManager.Singleton.LocalClientId)
                {
                    Debug.Log("¡Conexion aprobada por el servidor!");
                    tcs.TrySetResult(true);
                }
            }

            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            if (!NetworkManager.Singleton.StartClient())
            {
                Debug.LogWarning("StartClient() fallo al iniciar.");
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                return false;
            }

            Task timeoutTask = Task.Delay(5000);
            Task completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;

            if (completedTask == timeoutTask)
            {
                Debug.LogWarning("Timeout esperando respuesta del servidor.");
                NetworkManager.Singleton.Shutdown();
                return false;
            }

            bool isApproved = tcs.Task.Result;

            if (!isApproved)
            {
                try { await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId); } catch { }
                currentLobby = null;
                NetworkManager.Singleton.Shutdown();
            }

            return isApproved;
        }
        catch (Exception e)
        {
            Debug.LogWarning("JoinLobby failed: " + e);
            if (currentLobby != null)
            {
                try { await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId); } catch { }
                currentLobby = null;
            }
            return false;
        }
    }

    void ConfigureTransport(string ip, ushort port)
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, port);
    }

    async Task HeartbeatLobby()
    {
        if (currentLobby != null && NetworkManager.Singleton.IsServer)
        {
            try { await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id); }
            catch { }
        }
    }

    public async Task UpdateLobbyState(string newState)
    {
        if (currentLobby == null || !NetworkManager.Singleton.IsServer) return;

        try
        {
            await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "state", new DataObject(DataObject.VisibilityOptions.Public, newState, DataObject.IndexOptions.S1) }
                }
            });
        }
        catch (Exception e) { Debug.LogWarning("Failed to update lobby state: " + e.Message); }
    }

    // Solo el server deberia acceder a esta funcion
    // Solo el server deberia acceder a esta funcion
    public async Task CloseLobyAndShutdown()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;

        CancelInvoke(nameof(SendHeartbeatWrapper));

        if (currentLobby != null)
        {
            string lobbyIdToDelete = currentLobby.Id;

            currentLobby = null;

            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(lobbyIdToDelete);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error leaving lobby: " + e.Message);
            }
        }

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            await Task.Yield();
        }
    }
}