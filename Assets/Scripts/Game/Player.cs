using Unity.Netcode;
using System.Linq;
using UnityEngine;
using Unity.Collections;
using TypTyp;
using TypTyp.TextSystem;

public class Player : NetworkBehaviour
{
    //public int PlayerID;
    public MatchManager MatchManager { get; private set; }
    public RitualManager RitualManager { get; private set; }
    public ManaGainManager ManaManager { get; private set; }
    public CorruptionGainManager CorruptionManager { get; private set; }
    public DeckController DeckController { get; private set; }
    public SpellCaster SpellCaster { get; private set; }
    public StatusEffectController StatusEffectController { get; private set; }
    public CardUIManager CardUIManager { get; private set; }
    public PlayerInputManager PlayerInputManager { get; private set; }
    public ITextPipeline TextPipeline { get; private set; }
    public static Player User { get; private set; } //Acceso global al Player del jugador
    public static Player Enemy { get; private set; } //Acceso global al Player del enemigo

    //Las NetworkVariables de los jugadores est�n todas en el script Player, para su acceso 
    //intuitivo y NetworkBehaviour centralizado
    public NetworkVariable<float> RitualProgress { get; private set; } = new();
    public NetworkVariable<float> CurrentMana { get; private set; } = new();
    public NetworkVariable<float> CurrentCorruption { get; private set; } = new();

    private void Awake()
    {
        //Componentes del player
        GetComponents();

        //Componentes de escena
        MatchManager = FindFirstObjectByType<MatchManager>();
    }

    public override void OnNetworkSpawn()
    {
        RitualManager.enabled = IsOwner; //El ritual lo controla el cliente, evita mala UX por lag
        ManaManager.enabled = IsServer; //La ganancia de man� la controla el server exclusivamente
        CorruptionManager.enabled = IsServer; //La corrupci�n igual que el man�
        CardUIManager.enabled = IsOwner;

        if (IsOwner)
        {
            User = this; //Asignar user aqui permite utilizarlo en el Start de otros objetos del player
            // NOTA: el nombre un futuro se puede obtener de los datos del jugador desde los servicios de Unity
            FixedString32Bytes PlayerName = PlayerPrefs.GetString("Username");
            Debug.Log(PlayerName);
            int[] deck = CardRegister.Instance.GetIds(DeckBuilder.CardsInDeck);
            PlayerData playerData = new(OwnerClientId, PlayerName, deck);

            MatchManager.OnPlayerReadyRpc(playerData);
            RitualManager.OnProgressUpdated += (p) => UpdateRitualProgressRpc(p);
        }
        if (IsServer)
        {
            ManaManager.OnManaGain += (m) => UpdateCurrentMana(m);
            CorruptionManager.OnCorruptionGain += (c) => UpdateCurrentCorruption(c);
        }
    }

    public void ConfigureServerPlayer(PlayerData playerData)
    {
        if (!IsServer) return;
        DeckController.ConfigureServerDeckController(playerData.Deck);
    }

    //Esto no llega al server en caso de un servidor dedicado
    //el enabled de los componentes daria error
    //solo funciona porque nuestro juego se supone para server hosts
    [Rpc(SendTo.ClientsAndHost)]
    public void ConfigurePlayerRpc(int playerIdx)
    {
        //PlayerID = playerIdx == 0 ? TypTyp.Settings.Instance.P1_ID : TypTyp.Settings.Instance.P2_ID;
        this.tag = playerIdx == 0 ? Settings.Instance.P1_tag : Settings.Instance.P2_tag;

        FindFirstObjectByType<PlayerPositioner>().PositionPlayer(this, playerIdx, IsOwner);
        // RitualManager.enabled = IsOwner; //El ritual lo controla el cliente, evita mala UX por lag
        // ManaManager.enabled = IsServer; //La ganancia de man� la controla el server exclusivamente
        // CorruptionManager.enabled = IsServer; //La corrupci�n igual que el man�

        if (IsOwner)
        {
            Enemy = FindObjectsByType<Player>(FindObjectsSortMode.None).First(p => p != this);
            // User = this;

            FindFirstObjectByType<MatchManager>().NotifyConfiguredServerRpc(SetupType.Player);
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
        if (value == Settings.Instance.MaxCorruption)
        {
            Player winner = MatchManager.GetOpponent(OwnerClientId);

            if (winner != null)
            {
                MatchManager.HandlePlayerVictory(winner);
            }
        }
    }

    #region Helper Methods

    void GetComponents()
    {
        RitualManager = GetComponentInChildren<RitualManager>();
        ManaManager = GetComponent<ManaGainManager>();
        CorruptionManager = GetComponent<CorruptionGainManager>();
        DeckController = GetComponent<DeckController>();
        SpellCaster = GetComponent<SpellCaster>();
        StatusEffectController = GetComponent<StatusEffectController>();
        CardUIManager = GetComponentInChildren<CardUIManager>();
        PlayerInputManager = GetComponent<PlayerInputManager>();
        TextPipeline = GetComponent<ITextPipeline>();

        UnityEngine.Assertions.Assert.IsNotNull(RitualManager);
        UnityEngine.Assertions.Assert.IsNotNull(ManaManager);
        UnityEngine.Assertions.Assert.IsNotNull(CorruptionManager);
        UnityEngine.Assertions.Assert.IsNotNull(DeckController);
        UnityEngine.Assertions.Assert.IsNotNull(SpellCaster);
        UnityEngine.Assertions.Assert.IsNotNull(StatusEffectController);
        UnityEngine.Assertions.Assert.IsNotNull(CardUIManager);
        UnityEngine.Assertions.Assert.IsNotNull(PlayerInputManager);
    }
    #endregion
}
