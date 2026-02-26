using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using LobbyPlayer = Unity.Services.Lobbies.Models.Player;

// Gestiona el matchmaking usando Unity Lobby + Relay + Netcode for GameObjects
public class LobbyManager : MonoBehaviour
{
    // Número máximo de jugadores permitidos en la lobby
    const int MaxPlayers = 2;

    // Tiempo máximo para intentar reconectar (segundos)
    [SerializeField] private float maxReconnectTime = 10f;

    // Intervalo entre intentos de reconexión (segundos)
    [SerializeField] private float reconnectInterval = 2f;

    // Intervalo del heartbeat de la lobby (segundos)
    [SerializeField] private float heartbeatInterval = 15f;

    // Referencia a la lobby actual (si somos host o cliente)
    Lobby currentLobby;

    bool isMatching;
    bool isReconnecting;

    // Se ejecuta al iniciar el objeto
    async void Awake()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

        _ = SearchLobby();
    }

    // Método público para hacer "Quick Play"
    // Busca una lobby existente o crea una nueva si no hay ninguna
    public async Task SearchLobby()
    {
        if (isMatching) return;
        isMatching = true;

        await Task.Delay(UnityEngine.Random.Range(500, 1500));

        try
        {
            QueryResponse query = await LobbyService.Instance.QueryLobbiesAsync(
                new QueryLobbiesOptions
                {
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(
                            QueryFilter.FieldOptions.AvailableSlots,
                            "0",
                            QueryFilter.OpOptions.GT)
                    }
                });

            // Si hay alguna lobby creada, nos unimos a la primera
            if (query.Results.Count > 0)
            {
                await JoinLobby(query.Results[0]);
                return;
            }

            // Si no hay ninguna, creamos una nueva
            await CreateLobby();
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
        {
            Debug.LogWarning("Rate limited. Waiting before retry...");

            await Task.Delay(5000); // espera 5 segundos

            await SearchLobby();
            return;
        }
        catch (Exception e)
        {
            Debug.LogError("Matchmaking failed: " + e);
        }
        finally
        {
            isMatching = false;
        }
    }

    // Crea una lobby y configura Relay como host
    async Task CreateLobby()
    {
        // Crea una asignación de Relay
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers - 1);

        // Obtiene el código que usarán los clientes para unirse
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        // Datos persistentes del jugador (sessionId = PlayerId)
        var playerData = new Dictionary<string, PlayerDataObject>
        {
            {
                "sessionId",
                new PlayerDataObject(
                    PlayerDataObject.VisibilityOptions.Member,
                    AuthenticationService.Instance.PlayerId)
            }
        };

        // Opciones de la lobby
        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = new LobbyPlayer(data: playerData),
            Data = new Dictionary<string, DataObject>
            {
                // Guardamos el joinCode dentro de la lobby
                { "joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
            }
        };

        // Crea la lobby
        currentLobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", MaxPlayers, options);

        // Conecta al Relay como host
        ConfigureTransport(allocation);

        if (!NetworkManager.Singleton.StartHost())
        {
            Debug.LogError("Failed to start host");
            return;
        }

        // Esperamos un momento para detectar conflictos
        await Task.Delay(2000);

        // Resolución determinista de conflicto
        await ResolveHostConflict();

        InvokeRepeating(nameof(HeartbeatLobby), heartbeatInterval, heartbeatInterval);
    }

    // Detecta si existen dos hosts y resuelve el conflicto
    async Task ResolveHostConflict()
    {
        QueryResponse query = await LobbyService.Instance.QueryLobbiesAsync(
            new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(
                        QueryFilter.FieldOptions.AvailableSlots,
                        "0",
                        QueryFilter.OpOptions.GT)
                }
            });

        if (query.Results.Count <= 1)
            return;

        // Ordenamos por ID para decisión determinista
        query.Results.Sort((a, b) => string.Compare(a.Id, b.Id, StringComparison.Ordinal));

        Lobby winningLobby = query.Results[0];

        if (currentLobby.Id != winningLobby.Id)
        {
            // Perdemos el conflicto -> apagamos host y nos unimos al ganador
            NetworkManager.Singleton.Shutdown();

            await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);

            currentLobby = null;

            // Recargar escena limpia antes de reconectar
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
        // Si somos el ganador, simplemente seguimos como host
    }

    // Se une a una lobby existente y configura Relay como cliente
    async Task JoinLobby(Lobby lobby)
    {
        var playerData = new Dictionary<string, PlayerDataObject>
        {
            {
                "sessionId",
                new PlayerDataObject(
                    PlayerDataObject.VisibilityOptions.Member,
                    AuthenticationService.Instance.PlayerId)
            }
        };

        currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(
            lobby.Id,
            new JoinLobbyByIdOptions
            {
                Player = new LobbyPlayer(data: playerData)
            });

        await ConnectToRelay();
    }

    // Conecta al Relay usando el joinCode guardado en la lobby
    async Task ConnectToRelay()
    {
        if (currentLobby == null || !currentLobby.Data.ContainsKey("joinCode"))
        {
            Debug.LogError("Invalid lobby data");
            GoToMainMenu();
            return;
        }

        string joinCode = currentLobby.Data["joinCode"].Value;

        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        ConfigureTransport(allocation);

        if (!NetworkManager.Singleton.StartClient())
        {
            Debug.LogError("Failed to start client");
            GoToMainMenu();
        }
    }

    // Configura UnityTransport con datos de Relay
    void ConfigureTransport(Allocation allocation)
    {
        RelayServerData relayData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);
    }

    // Overload para JoinAllocation
    void ConfigureTransport(JoinAllocation allocation)
    {
        RelayServerData relayData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);
    }

    async void HeartbeatLobby()
    {
        if (currentLobby != null && NetworkManager.Singleton.IsHost)
        {
            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Heartbeat failed: " + e.Message);
            }
        }
    }

    private async void HandleClientDisconnect(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
            return;

        if (isReconnecting)
            return;

        isReconnecting = true;

        float elapsed = 0f;

        while (elapsed < maxReconnectTime)
        {
            await Task.Delay(TimeSpan.FromSeconds(reconnectInterval));
            elapsed += reconnectInterval;

            try
            {
                if (currentLobby == null)
                {
                    GoToMainMenu();
                    isReconnecting = false;
                    return;
                }

                var lobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

                if (lobby == null)
                {
                    GoToMainMenu();
                    isReconnecting = false;
                    return;
                }

                currentLobby = lobby;

                if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsConnectedClient)
                    NetworkManager.Singleton.Shutdown();

                await ConnectToRelay();

                if (NetworkManager.Singleton.IsConnectedClient)
                {
                    isReconnecting = false;
                    return;
                }
            }
            catch
            {
            }
        }

        isReconnecting = false;
        GoToMainMenu();
    }

    void GoToMainMenu()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }

    async void OnApplicationQuit()
    {
        if (currentLobby != null && NetworkManager.Singleton.IsHost)
            await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
    }

    void OnDestroy()
    {
        CancelInvoke(nameof(HeartbeatLobby));

        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }
}