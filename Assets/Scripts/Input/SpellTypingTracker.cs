using Unity.Netcode;
using Unity.Collections;
using UnityEngine;
using TypTyp; // Añadido para acceder a Settings.Instance

[RequireComponent(typeof(Player))]
public class SpellTypingTracker : NetworkBehaviour
{
    #region varaibles de texto
    // Network variables que escribe el owner
    [Tooltip("Lo que escribe el jugador.")]
    public NetworkVariable<FixedString32Bytes> RawText = new NetworkVariable<FixedString32Bytes>(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    [Tooltip("El texto filtrado.")]
    public NetworkVariable<FixedString32Bytes> FilteredText = new NetworkVariable<FixedString32Bytes>(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    [Tooltip("Sincroniza si este jugador tiene activado el chat completo, permitiendo a otros leerlo.")]
    public NetworkVariable<bool> AllowRawChat = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    // Locales
    public event System.Action<string> OnLocalRawTextChanged;
    public event System.Action<string> OnLocalFilteredTextChanged;

    public string CurrentLocalRawText { get; private set; } = "";
    public string CurrentLocalFilteredText { get; private set; } = "";

    #endregion

    // Other varaibles
    private Player player;
    private CardUIManager cardUIManager;
    private bool isCastingSpells = false;
    private const int MAX_CHARS = 20;   // Modificar el tamaño máximo del chat


    private void Awake()
    {
        player = GetComponent<Player>();
        cardUIManager = GetComponentInChildren<CardUIManager>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Comunicar el setting del chat
            AllowRawChat.Value = Settings.Instance.ChatActive;

            // Eventos
            InputHandler.Instance.AddListener(OnCharTyped);
            player.PlayerInputManager.OnInputModeChangedEvent += OnInputModeChanged;
        }

        if (IsServer)
        {
            RawText.OnValueChanged += ProcessFilteringOnServer;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            if (InputHandler.Instance != null)
            {
                InputHandler.Instance.RemoveListener(OnCharTyped);
            }

            if (player != null && player.PlayerInputManager != null)
            {
                player.PlayerInputManager.OnInputModeChangedEvent -= OnInputModeChanged;
            }
        }

        if (IsServer)
        {
            RawText.OnValueChanged -= ProcessFilteringOnServer;
        }
    }

    private void OnInputModeChanged(InputMode mode)
    {
        isCastingSpells = (mode == InputMode.CastingSpells);
    }

    private void OnCharTyped(char c)
    {
        if (!isCastingSpells) return;
        if (c == '\n' || c == '\r') return;

        string current = RawText.Value.ToString();
        if (current.Length >= MAX_CHARS)
        {
            current = current.Substring(1);
        }

        current += c;

        RawText.Value = current;
        UpdateLocalTexts(current);
    }

    private void UpdateLocalTexts(string rawString)
    {
        // Local crudo
        CurrentLocalRawText = rawString;
        OnLocalRawTextChanged?.Invoke(CurrentLocalRawText);

        // Local filtrado (con filtrado local)
        CurrentLocalFilteredText = CalculateFilteredText(rawString);
        OnLocalFilteredTextChanged?.Invoke(CurrentLocalFilteredText);
    }

    // Función de filtrado
    private string CalculateFilteredText(string rawString)
    {
        if (string.IsNullOrEmpty(rawString)) return "";

        string matchedText = "";
        if (cardUIManager == null) return "";

        for (int i = 0; i < rawString.Length; i++)
        {
            string possiblePrefix = rawString.Substring(i);
            bool matchFound = false;

            // Obtener cartas y filtrar
            foreach (string spellName in cardUIManager.GetHandSpellNames())
            {
                if (spellName.StartsWith(possiblePrefix, System.StringComparison.Ordinal))
                {
                    matchedText = possiblePrefix;
                    matchFound = true;
                    break;
                }
            }

            if (matchFound) break;
        }

        return matchedText;
    }

    // Filtrado en red
    private void ProcessFilteringOnServer(FixedString32Bytes previous, FixedString32Bytes current)
    {
        FilteredText.Value = CalculateFilteredText(current.ToString());
    }
}