using Unity.Netcode;
using System.Linq;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private MatchManager matchManager;
    private RitualManager ritualManager;
    private ManaGainManager manaManager;
    public static Player User { get; private set; } //Acceso global al Player del jugador
    public static Player Enemy { get; private set; } //Acceso global al Player del enemigo

    //Las NetworkVariables de los jugadores están todas en el script Player, para su acceso 
    //intuitivo y NetworkBehaviour centralizado
    public NetworkVariable<float> RitualProgress { get; private set; } = new(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server
    );

    public NetworkVariable<float> CurrentMana { get; private set; } = new(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        ritualManager = GetComponentInChildren<RitualManager>();
        ritualManager.OnProgressUpdated += (p) => UpdateRitualProgressRpc(p);
        manaManager = GetComponentInChildren<ManaGainManager>();
        manaManager.OnManaGain += (m) => UpdateCurrentManaRpc(m);
        matchManager = FindFirstObjectByType<MatchManager>();
    }

    public override void OnNetworkSpawn()
    {
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
        ritualManager.enabled = IsOwner; //El ritual lo controla el cliente, evita mala UX por lag
        manaManager.enabled = IsServer; //La ganancia de maná la controla el server exclusivamente
    }

    [Rpc(SendTo.Server)]
    private void UpdateRitualProgressRpc(float progress)
    {
        //Como el ritual lo gestiona el cliente, en este método se debería agregar
        //prevención de trampas antes de validar el progreso proporcionado
        RitualProgress.Value = progress;
        if (progress >= 1.0) Debug.Log("Falta condición de finalización de partida.");
    }

    [Rpc(SendTo.Server)]
    private void UpdateCurrentManaRpc(float value) => CurrentMana.Value = value;
}
