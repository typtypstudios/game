using Unity.Netcode;
using System.Linq;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private MatchManager matchManager;
    private RitualManager ritualManager;
    private ManaGainManager manaManager;
    private CorruptionGainManager corruptionManager;
    public static Player User { get; private set; } //Acceso global al Player del jugador
    public static Player Enemy { get; private set; } //Acceso global al Player del enemigo

    //Las NetworkVariables de los jugadores están todas en el script Player, para su acceso 
    //intuitivo y NetworkBehaviour centralizado
    public NetworkVariable<float> RitualProgress { get; private set; } = new();
    public NetworkVariable<float> CurrentMana { get; private set; } = new();
    public NetworkVariable<float> CurrentCorruption { get; private set; } = new();

    private void Awake()
    {
        ritualManager = GetComponentInChildren<RitualManager>();
        manaManager = GetComponent<ManaGainManager>();
        corruptionManager = GetComponent<CorruptionGainManager>();
        matchManager = FindFirstObjectByType<MatchManager>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            matchManager.OnPlayerReadyRpc();
            ritualManager.OnProgressUpdated += (p) => UpdateRitualProgressRpc(p);
        }
        if (IsServer)
        {
            manaManager.OnManaGain += (m) => UpdateCurrentMana(m);
            corruptionManager.OnCorruptionGain += (c) => UpdateCurrentCorruption(c);
        }
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
        corruptionManager.enabled = IsServer; //La corrupción igual que el maná
    }

    [Rpc(SendTo.Server)]
    private void UpdateRitualProgressRpc(float progress)
    {
        if (RitualProgress.Value == progress)
        {
            corruptionManager.ProcessMistake();
            return;
        }
        //Como el ritual lo gestiona el cliente, en este método se debería agregar
        //prevención de trampas antes de validar el progreso proporcionado
        RitualProgress.Value = progress;
        if (progress >= 1.0) Debug.Log("Falta condición de finalización de partida.");
    }

    private void UpdateCurrentMana(float value) => CurrentMana.Value = value;

    private void UpdateCurrentCorruption(float value)
    {
        CurrentCorruption.Value = value;
        if (value == GlobalVariables.MaxCorruption) Debug.Log("Falta condición de derrota.");
    }
}
