using Unity.Netcode;
using System.Linq;
using UnityEngine;
using System;

public class Player : NetworkBehaviour
{
    private MatchManager matchManager; 
    public static Player User { get; private set; }
    public static Player Enemy { get; private set; }
    public RitualManager RitualManager => GetComponentInChildren<RitualManager>();

    public NetworkVariable<float> RitualProgress { get; private set; } = new(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        matchManager = FindFirstObjectByType<MatchManager>();
        if (IsOwner) matchManager.OnPlayerReadyRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ConfigurePlayerRpc(int playerIdx)
    {
        this.tag = playerIdx == 0 ? GlobalVariables.P1_tag : GlobalVariables.P2_tag;
        if (IsOwner)
        {
            Enemy = FindObjectsByType<Player>(FindObjectsSortMode.None).First(p => p != this);
            User = this;
        }
        FindFirstObjectByType<PlayerPositioner>().PositionPlayer(this, playerIdx, IsOwner);
        RitualManager.enabled = IsOwner;
    }

    public void UpdateRitualProgress(float progress) => RitualProgress.Value = progress;
}
