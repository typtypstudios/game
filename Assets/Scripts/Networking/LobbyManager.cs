using System;
using System.Collections.Generic;
using System.Linq;
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

    // Intervalo del heartbeat de la lobby (segundos)
    [SerializeField] private float heartbeatInterval = 15f;

    // Referencia a la lobby actual (si somos host o cliente)
    Lobby currentLobby;
    bool isMatching;

    // Se ejecuta al iniciar el objeto
    async void Awake()
    {
        await InitializeServices();
        _ = SearchLobby();
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

    // Método público para hacer "Quick Play"
    // Busca una lobby existente o crea una nueva si no hay ninguna
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

                    // Intentar unirse
                    if (query.Results.Count > 0)
                    {
                        if (await JoinLobby(query.Results[0]))
                            return;
                    }

                    // Intentar crear
                    if (await CreateLobby())
                        return;
                }
                catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    Debug.LogWarning("Rate limited. Waiting before retry...");
                    await Task.Delay(1000);
                }
                catch (Exception e)
                {
                    Debug.LogError("Matchmaking failed: " + e);
                    await Task.Delay(1000);
                }
            }// End for

            Debug.LogError("Matchmaking exhausted. Returning to main menu.");
            await CloseLobyAndShutdown();
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
                Debug.LogError("Failed to start host");
                await CloseLobyAndShutdown();
                return false;
            }

            await Task.Delay(2000);

            bool stillHost = await ResolveHostConflict();
            if (!stillHost)
            {
                return false; // perdió el conflicto reintentar
            }

            InvokeRepeating(nameof(SendHeartbeatWrapper), heartbeatInterval, heartbeatInterval);

            return true; // éxito
        }
        catch (Exception e)
        {
            Debug.LogError("CreateLobby failed: " + e);
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
            Debug.Log("Uniendose a un lobby como cliente");

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
                Debug.LogError("Invalid lobby data");
                await CloseLobyAndShutdown();
                return false;
            }

            string joinCode = currentLobby.Data["joinCode"].Value;

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            ConfigureTransport(allocation);

            if (!NetworkManager.Singleton.StartClient())
            {
                Debug.LogError("Failed to start client");
                await CloseLobyAndShutdown();
                return false;
            }

            return true; // éxito
        }
        catch (Exception e)
        {
            Debug.LogError("JoinLobby failed: " + e);
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

    public async Task UpdateLobbyState(string newState)
    {
        if (currentLobby == null) return;

        try
        {
            await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id,
                new UpdateLobbyOptions
                {
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
}