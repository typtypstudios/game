using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class MatchManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    int numPlayersReady = 0;

    //De momento una lista de playerIds server side
    Dictionary<int, Player> playersById;
    public Player GetPlayerById(int id) => playersById.GetValueOrDefault(id);

    //Cambiar esto mas adelante para mas jugadores
    int MaxPlayers => 2;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        //El servidor es el que instancia el prefab del Player
        //para garantizar que su OnNetworkSpawn se haga antes:
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            var player = Instantiate(playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        };

        playersById = new();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void OnPlayerReadyRpc()
    {
        if (++numPlayersReady == MaxPlayers)
        {
            Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
            for (int i = 0; i < players.Length; i++)
            {
                playersById.Add(i, players[i]);
                players[i].ConfigurePlayerRpc(i);
            }
            ConfigureUIRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ConfigureUIRpc() => FindFirstObjectByType<GameUIConfigurator>().ConfigureUI();
}