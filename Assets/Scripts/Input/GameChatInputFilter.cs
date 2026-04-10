using Unity.Netcode;
using Unity.Collections;
using UnityEngine;
using TypTyp;
using TypTyp.Input;

[RequireComponent(typeof(Player))]
public class GameChatInputFilter : NetworkBehaviour
{
    #region varaibles de texto
    // Network variable que escribe el owner
    [Tooltip("Lo que escribe el jugador.")]
    public NetworkVariable<FixedString64Bytes> RawText = new NetworkVariable<FixedString64Bytes>(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    [Tooltip("El texto filtrado por el servidor.")]
    public NetworkVariable<FixedString64Bytes> FilteredText = new NetworkVariable<FixedString64Bytes>(
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
    private const int MAX_CHARS = 50;


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

        if (c == ' ' && current.EndsWith(" ")) return;

        string candidate = current + c;

        // Solo envolvemos con espacios si el chat sin filtrar está activo,
        // que es cuando el texto "raw" se ve realmente en pantalla.
        if (Settings.Instance.ChatActive)
            candidate = TryWrapSpellWithSpaces(candidate);

        while (candidate.Length > MAX_CHARS)
            candidate = candidate.Substring(1);

        try { RawText.Value = candidate; }
        catch (System.ArgumentException) { return; }

        UpdateLocalTexts(candidate);
    }

    /// <summary>
    /// Si el final del texto coincide con el nombre completo de un spell de la mano,
    /// añade un espacio a la izquierda (solo si hay algo antes y no es ya un espacio)
    /// y un espacio a la derecha. Nunca genera dos espacios seguidos.
    /// </summary>
    private string TryWrapSpellWithSpaces(string candidate)
    {
        if (cardUIManager == null || string.IsNullOrEmpty(candidate))
            return candidate;

        foreach (string spellName in cardUIManager.GetHandSpellNames())
        {
            if (string.IsNullOrEmpty(spellName)) continue;
            if (!candidate.EndsWith(spellName, System.StringComparison.Ordinal)) continue;

            int spellStart = candidate.Length - spellName.Length;
            string before = candidate.Substring(0, spellStart);

            // Espacio izquierdo: solo si hay texto antes y no termina ya en espacio
            if (before.Length > 0 && !before.EndsWith(" "))
                candidate = before + " " + spellName;

            // Espacio derecho: siempre (la regla de "no dos espacios seguidos"
            // en OnCharTyped impedirá que el usuario meta otro espacio detrás).
            candidate += " ";
            break;
        }

        return candidate;
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
        if (cardUIManager == null) return "";

        for (int i = 0; i < rawString.Length; i++)
        {
            if (rawString[i] == ' ') continue; // ignorar espacios a la izquierda

            string possiblePrefix = rawString.Substring(i).TrimEnd(' '); // ignorar espacio derecho
            if (possiblePrefix.Length == 0) continue;

            foreach (string spellName in cardUIManager.GetHandSpellNames())
            {
                if (spellName.StartsWith(possiblePrefix, System.StringComparison.Ordinal))
                    return possiblePrefix;
            }
        }
        return "";
    }

    // Filtrado en red (Servidor)
    private void ProcessFilteringOnServer(FixedString64Bytes previous, FixedString64Bytes current)
    {
        // El server se encarga de filtrar el texto que le llega para solo mostrar el texto que coincida con las cartas que está escribiendo
        FilteredText.Value = CalculateFilteredTextServer(current.ToString());
    }

    private string CalculateFilteredTextServer(string rawString)
    {
        if (string.IsNullOrEmpty(rawString)) return "";
        if (deckController == null || deckController.CurrentHand == null) return "";

        for (int i = 0; i < rawString.Length; i++)
        {
            if (rawString[i] == ' ') continue;

            string possiblePrefix = rawString.Substring(i).TrimEnd(' ');
            if (possiblePrefix.Length == 0) continue;

            foreach (int cardId in deckController.CurrentHand)
            {
                var cardDef = CardRegister.Instance.GetById(cardId);
                if (cardDef != null && cardDef.name.StartsWith(possiblePrefix, System.StringComparison.Ordinal))
                    return possiblePrefix;
            }
        }
        return "";
    }
}