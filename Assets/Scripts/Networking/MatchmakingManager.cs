using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

// Gestiona el matchmaking usando Unity Lobby + Relay + Netcode for GameObjects
public class MatchmakingManager : MonoBehaviour
{
    // Número máximo de jugadores permitidos en la lobby
    const int MaxPlayers = 2;

    // Referencia a la lobby actual (si somos host o cliente)
    Lobby currentLobby;

    // Se ejecuta al iniciar el objeto
    async void Awake()
    {
        // Inicializa todos los servicios de Unity Gaming Services (Lobby, Relay, etc.)
        await UnityServices.InitializeAsync();

        // Inicia sesión anónima para poder usar los servicios online
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        QuickPlay();
    }

    // Método público para hacer "Quick Play"
    // Busca una lobby existente o crea una nueva si no hay ninguna
    public async void QuickPlay()
    {
        try
        {
            // Busca lobbies disponibles
            var query = await LobbyService.Instance.QueryLobbiesAsync();

            // Si hay alguna lobby creada, nos unimos a la primera
            if (query.Results.Count > 0)
                await JoinLobby(query.Results[0]);
            else
                // Si no hay ninguna, creamos una nueva
                await CreateLobby();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Matchmaking failed: " + e);
        }
    }

    // Crea una lobby y configura Relay como host
    async Task CreateLobby()
    {
        // Crea una asignación de Relay para permitir conexiones
        // MaxPlayers - 1 porque el host ya cuenta como jugador
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers - 1);

        // Obtiene el código que usarán los clientes para unirse
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        // Opciones de la lobby
        var options = new CreateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                // Guardamos el joinCode dentro de la lobby
                // VisibilityOptions.Member significa que solo miembros de la lobby pueden verlo
                { "joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
            }
        };

        // Crea la lobby en el servicio de Lobby
        currentLobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", MaxPlayers, options);

        // Convierte la información de Relay en datos compatibles con UnityTransport
        var relayData = AllocationUtils.ToRelayServerData(allocation, "dtls");

        // Configura el transporte para usar Relay en lugar de conexión directa
        NetworkManager.Singleton
            .GetComponent<UnityTransport>()
            .SetRelayServerData(relayData);

        // Inicia el juego como Host (servidor + cliente local)
        NetworkManager.Singleton.StartHost();
    }

    // Se une a una lobby existente y configura Relay como cliente
    async Task JoinLobby(Lobby lobby)
    {
        // Nos unimos a la lobby usando su ID
        currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);

        // Recuperamos el joinCode guardado por el host
        string joinCode = currentLobby.Data["joinCode"].Value;

        // Nos conectamos al Relay usando ese código
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        // Convertimos los datos de Relay al formato que entiende UnityTransport
        var relayData = AllocationUtils.ToRelayServerData(allocation, "dtls");

        // Configuramos el transporte con esos datos
        NetworkManager.Singleton
            .GetComponent<UnityTransport>()
            .SetRelayServerData(relayData);

        // Iniciamos como cliente (nos conectamos al host)
        NetworkManager.Singleton.StartClient();
    }

    // Se ejecuta cuando se cierra la aplicación
    async void OnApplicationQuit()
    {
        // Si existe una lobby activa y somos host,
        // la eliminamos para que no quede "huérfana" en el servicio
        if (currentLobby != null)
            await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
    }
}