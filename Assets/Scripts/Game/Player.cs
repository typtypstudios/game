using Unity.Netcode;
using System.Linq;
using UnityEngine;
using Unity.Collections;
using TypTyp;
using TypTyp.TextSystem;
using TypTyp.Input;

public class Player : NetworkBehaviour
{
    public MatchManager MatchManager { get; private set; }
    public RitualManager RitualManager { get; private set; }
    public ManaGainManager ManaManager { get; private set; }
    public CorruptionGainManager CorruptionManager { get; private set; }
    public DeckController DeckController { get; private set; }
    public SpellCaster SpellCaster { get; private set; }
    public StatusEffectController StatusEffectController { get; private set; }
    public CardUIManager CardUIManager { get; private set; }
    public PlayerInputManager PlayerInputManager { get; private set; }
    public SpellTypingTracker SpellTypingTracker { get; private set; }
    public ITextPipeline TextPipeline { get; private set; }
    public static Player User { get; private set; }
    public static Player Enemy { get; private set; }

    public NetworkVariable<float> RitualProgress { get; private set; } = new();
    public NetworkVariable<float> CurrentMana { get; private set; } = new();
    public NetworkVariable<float> CurrentCorruption { get; private set; } = new();

    private void Awake()
    {
        GetComponents();
        MatchManager = FindFirstObjectByType<MatchManager>();
    }

    public override void OnNetworkSpawn()
    {
        RitualManager.enabled = IsOwner;
        ManaManager.enabled = IsServer;
        CorruptionManager.enabled = IsServer;
        CardUIManager.enabled = IsOwner;

        if (IsOwner)
        {
            User = this;

            string playerNameValue = "AverageCultist";
            if (SaveManager.Instance.HasLoadedState)
            {
                SaveState state = SaveManager.Instance.GetState();
                if (!string.IsNullOrWhiteSpace(state.slot.profile.username))
                {
                    playerNameValue = state.slot.profile.username;
                }
            }

            FixedString32Bytes playerName = playerNameValue;
            int[] deck = CardRegister.Instance.GetIds(DeckBuilder.CardsInDeck);
            PlayerData playerData = new(OwnerClientId, playerName, deck);

            InputHandler.Instance.SetMode(InputModeMask.WaitingForPlayers);
            MatchManager.OnPlayerReadyRpc(playerData);
            RitualManager.OnProgressUpdated += progress => UpdateRitualProgressRpc(progress);
        }

        if (IsServer)
        {
            ManaManager.OnManaGain += mana => UpdateCurrentMana(mana);
            CorruptionManager.OnCorruptionGain += corruption => UpdateCurrentCorruption(corruption);
        }
    }

    public void ConfigureServerPlayer(PlayerData playerData)
    {
        if (!IsServer) return;
        DeckController.ConfigureServerDeckController(playerData.Deck);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ConfigurePlayerRpc(int playerIdx)
    {
        this.tag = playerIdx == 0 ? Settings.Instance.P1_tag : Settings.Instance.P2_tag;

        FindFirstObjectByType<PlayerPositioner>().PositionPlayer(this, playerIdx, IsOwner);

        if (IsOwner)
        {
            Enemy = FindObjectsByType<Player>(FindObjectsSortMode.None).First(p => p != this);
            FindFirstObjectByType<MatchManager>().NotifyPlayerConfiguredServerRpc();
            // InputHandler.Instance.SetMode(InputModeMask.Ritual);
        }
    }

    [Rpc(SendTo.Server)]
    private void UpdateRitualProgressRpc(float progress)
    {
        //BUG, esto ocurre antes de iniciar partida
        //El player manda un RPC a server cada vez que updatea progress del Ritual
        //El progress se updatea cada vez que se actualiza el texto
        //El texto se actualiza al conectarse el jugador, al recibir los textos iniciales
        //En ese momento el ritual manda un RPC con progress 0, que es el valor inicial
        //y este codigo procesa error, una genialidad
        if (RitualProgress.Value == progress)
        {
            CorruptionManager.ProcessMistake();
            return;
        }

        RitualProgress.Value = progress;

        if (RitualProgress.Value >= 1f) MatchManager.HandlePlayerVictory(this);
    }

    private void UpdateCurrentMana(float value) => CurrentMana.Value = value;

    private void UpdateCurrentCorruption(float value)
    {
        CurrentCorruption.Value = value;
        if (value == Settings.Instance.MaxCorruption)
        {
            Debug.Log("Player " + name + " has reached max corruption and lost the match.", gameObject);
            Player winner = MatchManager.GetPlayerById(MatchManager.GetPlayerId(this) == 0 ? 1 : 0);
            MatchManager.HandlePlayerVictory(winner);
        }
    }

    private void GetComponents()
    {
        RitualManager = GetComponentInChildren<RitualManager>();
        ManaManager = GetComponent<ManaGainManager>();
        CorruptionManager = GetComponent<CorruptionGainManager>();
        DeckController = GetComponent<DeckController>();
        SpellCaster = GetComponent<SpellCaster>();
        StatusEffectController = GetComponent<StatusEffectController>();
        CardUIManager = GetComponentInChildren<CardUIManager>();
        PlayerInputManager = GetComponent<PlayerInputManager>();
        SpellTypingTracker = GetComponent<SpellTypingTracker>();
        TextPipeline = GetComponent<ITextPipeline>();

        UnityEngine.Assertions.Assert.IsNotNull(RitualManager);
        UnityEngine.Assertions.Assert.IsNotNull(ManaManager);
        UnityEngine.Assertions.Assert.IsNotNull(CorruptionManager);
        UnityEngine.Assertions.Assert.IsNotNull(DeckController);
        UnityEngine.Assertions.Assert.IsNotNull(SpellCaster);
        UnityEngine.Assertions.Assert.IsNotNull(StatusEffectController);
        UnityEngine.Assertions.Assert.IsNotNull(CardUIManager);
        UnityEngine.Assertions.Assert.IsNotNull(PlayerInputManager);
        UnityEngine.Assertions.Assert.IsNotNull(SpellTypingTracker);
    }
}
