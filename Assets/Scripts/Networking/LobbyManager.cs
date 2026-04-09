using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
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

    // Intervalo del heartbeat de la lobby (segundos)
    [SerializeField] private float heartbeatInterval = 15f;

    // Referencia a la lobby actual (si somos host o cliente)
    Lobby currentLobby;
    bool isMatching;
    private CancellationTokenSource lifetimeCts;

    // Se ejecuta al iniciar el objeto
    async void Awake()
    {
        lifetimeCts = new CancellationTokenSource();

        await InitializeServices();
        if (lifetimeCts.IsCancellationRequested) return;

        _ = SearchLobby();
    }

    async Task InitializeServices()
    {
        try
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
        catch (Exception e)
        {
            Debug.LogError("Failed to initialize Unity Services: " + e.Message);
            if (Application.isPlaying)
                SceneManager.LoadScene("MainMenu");
        }
    }

    // Método público para hacer "Quick Play"
    // Busca una lobby existente o crea una nueva si no hay ninguna
    public async Task SearchLobby()
    {
        if (isMatching) return;
        isMatching = true;

        var token = lifetimeCts?.Token ?? CancellationToken.None;

        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogWarning("Not signed in, aborting lobby search");
                return;
            }

            for (int i = 0; i < 5; ++i)
            {
                if (token.IsCancellationRequested) return;

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
                                QueryFilter.OpOptions.GT),
                            new QueryFilter(
                                QueryFilter.FieldOptions.S1,
                                "waiting",
                                QueryFilter.OpOptions.EQ)
                            }
                        });

                    if (token.IsCancellationRequested) return;

                    // Intentar unirse
                    if (query.Results.Count > 0)
                    {
                        if (await JoinLobby(query.Results[0]))
                            return;
                    }

                    if (token.IsCancellationRequested) return;

                    // Intentar crear
                    if (await CreateLobby())
                        return;
                }
                catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    Debug.Log("Rate limited. Waiting before retry...");
                    try { await Task.Delay(1000, token); }
                    catch (OperationCanceledException) { return; }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception e)
                {
                    if (token.IsCancellationRequested) return;
                    Debug.LogError("Matchmaking failed: " + e);
                    try { await Task.Delay(1000, token); }
                    catch (OperationCanceledException) { return; }
                }
            }// End for

            if (token.IsCancellationRequested) return;

            Debug.LogWarning("Matchmaking exhausted. Returning to main menu.");
            await CloseLobyAndShutdown();
            if (Application.isPlaying)
                SceneManager.LoadScene("MainMenu");
        }
        finally
        {
            isMatching = false;
        }
    }

    // Crea una lobby y configura Relay como host
    async Task<bool> CreateLobby()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers - 1, "europe-west4");
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var playerData = new Dictionary<string, PlayerDataObject>
            {
                {
                    "sessionId",
                    new PlayerDataObject(
                        PlayerDataObject.VisibilityOptions.Member,
                        AuthenticationService.Instance.PlayerId)
                }
            };

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Player = new LobbyPlayer(data: playerData),
                Data = new Dictionary<string, DataObject>
                {
                    { "joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) },
                    { "state", new DataObject(
                        DataObject.VisibilityOptions.Public,
                        "waiting",
                        DataObject.IndexOptions.S1)
                    }
                }
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", MaxPlayers, options);
            ConfigureTransport(allocation);

            if (!NetworkManager.Singleton.StartHost())
            {
                Debug.LogWarning("Failed to start host");
                await CloseLobyAndShutdown();
                return false;
            }

            await Task.Delay(2000);

            bool stillHost = await ResolveHostConflict();
            if (!stillHost)
            {
                return false; // perdió el conflicto reintentar
            }

            Debug.Log("Uniendose a un lobby como host");
            InvokeRepeating(nameof(SendHeartbeatWrapper), heartbeatInterval, heartbeatInterval);

            return true; // éxito
        }
        catch (Exception e)
        {
            Debug.LogWarning("CreateLobby failed: " + e);
            await CloseLobyAndShutdown();
            return false;
        }
    }

    void SendHeartbeatWrapper()
    {
        _ = HeartbeatLobby();
    }

    // Detecta si existen dos hosts y resuelve el conflicto
    async Task<bool> ResolveHostConflict()
    {
        QueryResponse query = await LobbyService.Instance.QueryLobbiesAsync(
        new QueryLobbiesOptions
        {
            Filters = new List<QueryFilter>
            {
                new QueryFilter(
                    QueryFilter.FieldOptions.AvailableSlots,
                    "0",
                    QueryFilter.OpOptions.GT),

                new QueryFilter(
                    QueryFilter.FieldOptions.S1,
                    "waiting",
                    QueryFilter.OpOptions.EQ)
            }
        });

        if (query.Results.Count <= 1)
            return true;

        query.Results.Sort((a, b) => string.Compare(a.Id, b.Id, StringComparison.Ordinal));

        Lobby winningLobby = query.Results[0];

        if (currentLobby == null)
        {
            Debug.LogWarning("Current lobby is null during host conflict resolution.");
            return false;
        }

        // Decide si se queda o se va del lobby
        if (currentLobby.Id != winningLobby.Id)
        {
            Debug.Log("ABANDONANDO ESTE LOBBY");

            await CloseLobyAndShutdown();

            await Task.Delay(500);

            return false;
        }

        return true;
    }

    // Se une a una lobby existente y configura Relay como cliente
    async Task<bool> JoinLobby(Lobby lobby)
    {
        try
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

            if (currentLobby == null || !currentLobby.Data.ContainsKey("joinCode"))
            {
                Debug.LogWarning("Invalid lobby data");
                await CloseLobyAndShutdown();
                return false;
            }

            string joinCode = currentLobby.Data["joinCode"].Value;

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            ConfigureTransport(allocation);

            if (!NetworkManager.Singleton.StartClient())
            {
                Debug.LogWarning("Failed to start client");
                await CloseLobyAndShutdown();
                return false;
            }

            Debug.Log("Uniendose a un lobby como cliente");
            return true; // éxito
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.LobbyConflict)
        {
            Debug.LogWarning($"Conflicto 409: Ya figuramos en la lobby {lobby.Id}. Forzando salida para limpiar sesión...");

            try
            {
                // intentar salir de este lobby
                await LobbyService.Instance.RemovePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId);
                Debug.Log("Salida forzada exitosa. El bucle de búsqueda intentará con otra lobby o creará una.");
            }
            catch (Exception removeEx)
            {
                Debug.LogError($"No se pudo limpiar la sesión en la lobby: {removeEx.Message}");
            }

            currentLobby = null;
            return false;
        }
        catch (Exception e)
        {
            Debug.LogWarning("JoinLobby failed: " + e);
            await CloseLobyAndShutdown();
            return false;
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

    async Task HeartbeatLobby()
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

    public async Task UpdateLobbyState(string newState, bool locked = false)
    {
        if (currentLobby == null) return;

        try
        {
            await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id,
                new UpdateLobbyOptions
                {
                    IsLocked = locked,
                    Data = new Dictionary<string, DataObject>
                    {
                    {
                        "state",
                        new DataObject(
                            DataObject.VisibilityOptions.Public,
                            newState,
                            DataObject.IndexOptions.S1)
                    }
                    }
                });
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to update lobby state: " + e.Message);
        }
    }


    public async Task CloseLobyAndShutdown()
    {
        CancelInvoke(nameof(SendHeartbeatWrapper));

        bool wasHost = NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost;

        if (NetworkManager.Singleton != null &&
            (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient))
        {
            NetworkManager.Singleton.Shutdown();
            await Task.Yield();
        }

        if (currentLobby != null)
        {
            try
            {
                if (wasHost)
                {
                    await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
                }
                else
                {
                    await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error leaving lobby: " + e.Message);
            }

            currentLobby = null;
        }
    }

    private void OnDestroy()
    {
        if (lifetimeCts != null)
        {
            lifetimeCts.Cancel();
            lifetimeCts.Dispose();
            lifetimeCts = null;
        }
    }
}