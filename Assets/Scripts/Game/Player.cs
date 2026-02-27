using Unity.Netcode;
using System.Linq;
using UnityEngine;
using TypTyp;

public class Player : NetworkBehaviour
{
    public int PlayerID;
    public MatchManager MatchManager { get; private set; }
    public RitualManager RitualManager { get; private set; }
    public ManaGainManager ManaManager { get; private set; }
    public CorruptionGainManager CorruptionManager { get; private set; }
    public static Player User { get; private set; } //Acceso global al Player del jugador
    public static Player Enemy { get; private set; } //Acceso global al Player del enemigo

    //Las NetworkVariables de los jugadores est�n todas en el script Player, para su acceso 
    //intuitivo y NetworkBehaviour centralizado
    public NetworkVariable<float> RitualProgress { get; private set; } = new();
    public NetworkVariable<float> CurrentMana { get; private set; } = new();
    public NetworkVariable<float> CurrentCorruption { get; private set; } = new();

    private void Awake()
    {
        RitualManager = GetComponentInChildren<RitualManager>();
        ManaManager = GetComponent<ManaGainManager>();
        CorruptionManager = GetComponent<CorruptionGainManager>();
        MatchManager = FindFirstObjectByType<MatchManager>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            MatchManager.OnPlayerReadyRpc();
            RitualManager.OnProgressUpdated += (p) => UpdateRitualProgressRpc(p);
        }
        if (IsServer)
        {
            ManaManager.OnManaGain += (m) => UpdateCurrentMana(m);
            CorruptionManager.OnCorruptionGain += (c) => UpdateCurrentCorruption(c);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ConfigurePlayerRpc(int playerIdx)
    {
        PlayerID = playerIdx == 0 ? TypTyp.Settings.Instance.P1_ID : TypTyp.Settings.Instance.P2_ID;
        this.tag = playerIdx == 0 ? Settings.Instance.P1_tag : Settings.Instance.P2_tag;

        FindFirstObjectByType<PlayerPositioner>().PositionPlayer(this, playerIdx, IsOwner);
        RitualManager.enabled = IsOwner; //El ritual lo controla el cliente, evita mala UX por lag
        ManaManager.enabled = IsServer; //La ganancia de man� la controla el server exclusivamente
        CorruptionManager.enabled = IsServer; //La corrupci�n igual que el man�

        if (IsOwner)
        {
            Enemy = FindObjectsByType<Player>(FindObjectsSortMode.None).First(p => p != this);
            User = this;

            FindFirstObjectByType<MatchManager>().NotifyPlayerConfiguredServerRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void UpdateRitualProgressRpc(float progress)
    {
        if (RitualProgress.Value == progress)
        {
            CorruptionManager.ProcessMistake();
            return;
        }
        //Como el ritual lo gestiona el cliente, en este método se debería agregar
        //prevención de trampas antes de validar el progreso proporcionado
        RitualProgress.Value = progress;

        // Servidor comprueba si alguien ha llegado al final
        if (RitualProgress.Value >= 1f) MatchManager.HandlePlayerVictory(this);
    }

    private void UpdateCurrentMana(float value) => CurrentMana.Value = value;

    private void UpdateCurrentCorruption(float value)
    {
        CurrentCorruption.Value = value;
        if (value == Settings.Instance.MaxCorruption) Debug.Log("Falta condici�n de derrota.");
    }
}
