using Unity.Netcode;
using Unity.Collections;
using UnityEngine;
using TypTyp;
using TypTyp.Input;
using System.Collections.Generic;

[RequireComponent(typeof(Player))]
public class SpellTypingTracker : NetworkBehaviour
{
    #region varaibles de texto
    // Network variable que escribe el owner
    [Tooltip("Lo que escribe el jugador.")]
    public NetworkVariable<FixedString32Bytes> RawText = new NetworkVariable<FixedString32Bytes>(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    [Tooltip("El texto filtrado por el servidor.")]
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
    private DeckController deckController;
    private bool isCastingSpells = false;
    private const int MAX_CHARS = 20;


    private void Awake()
    {
        player = GetComponent<Player>();
        cardUIManager = GetComponentInChildren<CardUIManager>();
        deckController = GetComponent<DeckController>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Comunicar el setting del chat
            AllowRawChat.Value = Settings.Instance.ChatActive;

            // Eventos
            InputHandler.Instance.AddListener(OnCharTyped);
            InputHandler.Instance.OnInputModeChanged += OnInputModeChanged;
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
                InputHandler.Instance.OnInputModeChanged -= OnInputModeChanged;
            }

            if (player != null && player.PlayerInputManager != null)
            {
                // Input mode is handled by InputHandler now
            }
        }

        if (IsServer)
        {
            RawText.OnValueChanged -= ProcessFilteringOnServer;
        }
    }

    private void OnInputModeChanged(InputModeMask mode)
    {
        bool wasCasting = isCastingSpells;
        isCastingSpells = (mode == InputModeMask.Spells);

        if (isCastingSpells && !wasCasting && IsOwner)
        {
            RawText.Value = "";
            UpdateLocalTexts("");
        }
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

    /// <summary>
    /// Updates User's chat, locally. If they have the chat setting off, it 
    /// filters their chats locally looking at the cardUIManager cards.
    /// </summary>
    /// <param name="rawString">New text</param>
    private void UpdateLocalTexts(string rawString)
    {
        // El cliente filtra en local para, en todo momento,
        // ver el texto lo más rápido posible en su pantalla, si necesidad de esperar a que el server revise su propio chat

        CurrentLocalRawText = rawString;
        OnLocalRawTextChanged?.Invoke(CurrentLocalRawText);
        // Filtrado local (usando la UI local)
        CurrentLocalFilteredText = CalculateFilteredTextLocal(rawString);
        OnLocalFilteredTextChanged?.Invoke(CurrentLocalFilteredText);
    }

    // Función de filtrado local (Cliente)
    private string CalculateFilteredTextLocal(string rawString)
    {
        if (string.IsNullOrEmpty(rawString)) return "";

        string matchedText = "";
        if (cardUIManager == null) return "";

        for (int i = 0; i < rawString.Length; i++)
        {
            string possiblePrefix = rawString.Substring(i);
            bool matchFound = false;

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

    // Filtrado en red (Servidor)
    private void ProcessFilteringOnServer(FixedString32Bytes previous, FixedString32Bytes current)
    {
        // El server se encarga de filtrar el texto que le llega para solo mostrar el texto que coincida con las cartas que está escribiendo
        FilteredText.Value = CalculateFilteredTextServer(current.ToString());
    }

    private string CalculateFilteredTextServer(string rawString)
    {
        if (string.IsNullOrEmpty(rawString)) return "";

        string matchedText = "";
        if (deckController == null || deckController.CurrentHand == null) return "";

        for (int i = 0; i < rawString.Length; i++)
        {
            string possiblePrefix = rawString.Substring(i);
            bool matchFound = false;

            // Obtener cartas desde el DeckController en el servidor y filtrar
            foreach (int cardId in deckController.CurrentHand)
            {
                var cardDef = CardRegister.Instance.GetById(cardId);
                if (cardDef != null && cardDef.name.StartsWith(possiblePrefix, System.StringComparison.Ordinal))
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
}