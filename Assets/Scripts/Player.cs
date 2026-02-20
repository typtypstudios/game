using Unity.Netcode;
using System.Linq;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private MatchManager matchManager;
    private Player enemy;

    public override void OnNetworkSpawn()
    {
        matchManager = FindFirstObjectByType<MatchManager>();
        if (IsOwner) matchManager.OnPlayerReadyRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ConfigurePlayerRpc(int playerIdx)
    {
        this.tag = playerIdx == 0 ? GlobalVariables.P1_tag : GlobalVariables.P2_tag;
        enemy = FindObjectsByType<Player>(FindObjectsSortMode.None).First(p => p != this);
        FindFirstObjectByType<PlayerPositioner>().PositionPlayer(this, playerIdx, IsOwner);
    }
}
