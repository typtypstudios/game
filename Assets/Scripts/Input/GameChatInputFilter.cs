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

    private string FilteredTextOnServer = "";

    #endregion

    // Other varaibles
    private CardUIManager cardUIManager;
    private DeckController deckController;
    private bool isCastingSpells = false;
    private const int MAX_CHARS = 50;
    private ChatMarkerFormatter marker;


    private void Awake()
    {
        cardUIManager = GetComponentInChildren<CardUIManager>();
        deckController = GetComponent<DeckController>();
        marker = new ChatMarkerFormatter();
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
    }

    private void OnCharTyped(char c)
    {
        if (!isCastingSpells) return;
        if (c == '\n' || c == '\r') return;

        string current = marker.Strip(RawText.Value.ToString());

        if (c == ' ' && current.EndsWith(" ")) return;

        string candidate = current + c;

        candidate = TryWrapSpellWithSpaces(candidate);

        while (candidate.Length > MAX_CHARS)
            candidate = candidate.Substring(1);

        candidate = ApplyMarkersToClosedSpells(candidate);

        try { RawText.Value = candidate; }
        catch (System.ArgumentException) { return; }

        UpdateLocalTexts(candidate);
    }

    private string TryWrapSpellWithSpaces(string candidate)
    {
        if (deckController == null || deckController.Cards == null || string.IsNullOrEmpty(candidate))
            return candidate;

        foreach (CardDefinition cardDef in deckController.Cards)
        {
            if (cardDef == null) continue;
            string spellName = cardDef.name;
            if (string.IsNullOrEmpty(spellName)) continue;
            if (!candidate.EndsWith(spellName, System.StringComparison.Ordinal)) continue;

            int spellStart = candidate.Length - spellName.Length;
            string before = candidate.Substring(0, spellStart);

            if (before.Length > 0 && !before.EndsWith(" "))
            {
                candidate = before + " " + spellName;
            }

            candidate += " ";
            break;
        }

        return candidate;
    }

    private string ApplyMarkersToClosedSpells(string plain)
    {
        if (string.IsNullOrEmpty(plain)) return plain ?? "";
        if (deckController == null || deckController.Cards == null) return plain;

        var sb = new System.Text.StringBuilder(plain.Length + 16);
        int i = 0;
        while (i < plain.Length)
        {
            if (plain[i] == ' ') { sb.Append(' '); i++; continue; }

            int spaceIdx = plain.IndexOf(' ', i);
            bool isClosed = spaceIdx >= 0;
            int end = isClosed ? spaceIdx : plain.Length;
            string word = plain.Substring(i, end - i);

            bool matched = false;
            if (isClosed)
            {
                foreach (CardDefinition cardDef in deckController.Cards)
                {
                    if (cardDef != null && cardDef.name == word) { matched = true; break; }
                }
            }

            if (matched)
                sb.Append(marker.SpellMarker).Append(word).Append(marker.SpellMarker);
            else
                sb.Append(word);

            i = end;
        }
        return sb.ToString();
    }

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
        return BuildFilteredText(rawString);
    }

    // Filtrado en red (Servidor)
    private void ProcessFilteringOnServer(FixedString64Bytes previous, FixedString64Bytes current)
    {
        // El server se encarga de filtrar el texto que le llega para solo mostrar el texto que coincida con las cartas que está escribiendo
        FilteredText.Value = CalculateFilteredTextServer(current.ToString());
    }

    private string CalculateFilteredTextServer(string rawString)
    {
        return BuildFilteredText(rawString);
    }

    private string BuildFilteredText(string rawString)
    {
        if (string.IsNullOrEmpty(rawString)) return "";
        if (deckController == null || deckController.Cards == null) return "";

        string clean = marker.Strip(rawString);
        var sb = new System.Text.StringBuilder();

        int i = 0;
        while (i < clean.Length)
        {
            if (clean[i] == ' ') { i++; continue; }

            string bestMatch = null;
            bool bestIsExact = false;

            foreach (CardDefinition cardDef in deckController.Cards)
            {
                if (cardDef == null) continue;
                string name = cardDef.name;
                if (string.IsNullOrEmpty(name)) continue;

                if (i + name.Length <= clean.Length &&
                    string.CompareOrdinal(clean, i, name, 0, name.Length) == 0)
                {
                    if (bestMatch == null || name.Length > bestMatch.Length)
                    {
                        bestMatch = name;
                        bestIsExact = true;
                    }
                }
            }

            if (bestMatch == null)
            {
                int maxLen = 0;
                int j = i;
                while (j < clean.Length && clean[j] != ' ')
                {
                    string attempt = clean.Substring(i, j - i + 1);
                    if (IsCardPrefix(attempt))
                    {
                        maxLen = attempt.Length;
                    }
                    else if (maxLen > 0)
                    {
                        break;
                    }
                    j++;
                }

                if (maxLen > 0)
                {
                    bestMatch = clean.Substring(i, maxLen);
                    bestIsExact = false;
                }
            }

            if (bestMatch != null)
            {
                if (sb.Length > 0) sb.Append(' ');
                sb.Append(bestIsExact ? marker.Wrap(bestMatch) : bestMatch);
                i += bestMatch.Length;
            }
            else
            {
                i++;
            }
        }

        return sb.ToString();
    }

    private bool IsCardPrefix(string partial)
    {
        if (string.IsNullOrEmpty(partial)) return false;
        foreach (CardDefinition cardDef in deckController.Cards)
            if (cardDef != null && cardDef.name.StartsWith(partial, System.StringComparison.Ordinal)) return true;
        return false;
    }
}