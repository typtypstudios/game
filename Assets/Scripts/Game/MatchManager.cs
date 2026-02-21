using UnityEngine;
using Unity.Netcode;

public class MatchManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    int numPlayersReady = 0;

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
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void OnPlayerReadyRpc()
    {
        if(++numPlayersReady == 2)
        {
            Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
            for(int i = 0; i < players.Length; i++)
            {
                players[i].ConfigurePlayerRpc(i);
            }
            ConfigureUIRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ConfigureUIRpc() => FindFirstObjectByType<GameUIConfigurator>().ConfigureUI();
}