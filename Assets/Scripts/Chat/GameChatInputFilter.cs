using Unity.Netcode;
using Unity.Collections;
using UnityEngine;
using TypTyp;
using TypTyp.Input;
using TypTyp.TextSystem.Typable;
using System;
using System.Collections.Generic;
using System.Text;

[RequireComponent(typeof(Player))]
public class GameChatInputFilter : NetworkBehaviour
{
    #region Network Variables
    public NetworkVariable<FixedString512Bytes> RawText = new("",
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<FixedString512Bytes> FilteredText = new("",
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<bool> AllowRawChat = new(false,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    #endregion

    // Varaibles de texto en local con sus respectivos eventos
    public string CurrentLocalRawText { get; private set; } = "";
    public string CurrentLocalFilteredText { get; private set; } = "";
    public event Action<string> OnLocalRawTextChanged;
    public event Action<string> OnLocalFilteredTextChanged;

    // Limitar la cantidad máxima de caracteres para evitar desbordamientos
    private const int MaxPlainChars = 200;

    // Referencias
    private DeckController deckController;  // Para acceder a los typable controllers de las cartas actuales
    private ChatMarkerFormatter marker; // Insertar marcadores para colorear el texto

    private bool castingSpellMode = false;  // Comprobar que está en el modo de hechizos
    private bool pendingModeSpace = false;  // Se busca meter un espacio

    // Stringbuilders para la gestión de los strings cambiando todo el rato.
    // Estos strings no tienen los marcadores de color, solo procesan el texto
    // raw y el filtrado para luego buscar los hechizos lanzados.
    private readonly StringBuilder ownerRaw = new(MaxPlainChars + 50);
    private readonly StringBuilder filteredPlain = new(MaxPlainChars + 50);

    // Estructurar y almacenar los hechizos para aplicarles color
    private struct CastSpell { public string Name; public int Start; public int Length; }
    private List<CastSpell> castSpell = new();
    private List<CastSpell> castSpellRaw = new();
    private Dictionary<int, Queue<string>> locallyTypedSpellsCache = new();

    // Lista para almacenar las cartas actuales en el grimorio y no crear una lista nueva
    // todo el rato
    private List<(TypableController tc, CardUI ui)> currentCardUIs;


    private void Awake()
    {
        // Inicializar componentes y suscribir al evento de jugar carta
        deckController = GetComponent<DeckController>();

        deckController.OnCardPlayedEvent += HandleCardPlayed;
        deckController.OnLocalSpellExactTCText += HandleLocalSpellTypedExact;
        deckController.OnCardPlayRequestFailed += HandleCardPlayFailed;

        marker = new ChatMarkerFormatter();

        currentCardUIs = new List<(TypableController tc, CardUI ui)>(TypTyp.Settings.Instance.HandSize);
    }

    private void HandleCardPlayFailed(CardEventArgs args)
    {
        if (locallyTypedSpellsCache.TryGetValue(args.CardId, out var queue) && queue.Count > 0)
        {
            queue.Dequeue();
        }
    }

    private void HandleLocalSpellTypedExact(int cardId, string exactText)
    {
        //Debug.Log("Exact text: " + exactText);
        if (!locallyTypedSpellsCache.ContainsKey(cardId))
        {
            locallyTypedSpellsCache[cardId] = new Queue<string>();
        }
        locallyTypedSpellsCache[cardId].Enqueue(exactText);
    }

    public override void OnNetworkSpawn()
    {
        // Configurar los listeners de input solo para el jugador local
        if (!IsOwner) return;

        AllowRawChat.Value = Settings.Instance.ChatActive;
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.AddPriorityListener(OnCharTyped);
            InputHandler.Instance.OnInputModeChanged += OnInputModeChanged;
            castingSpellMode = (InputHandler.Instance.CurrentMode == InputModeMask.Spells);
        }
    }

    public override void OnNetworkDespawn()
    {
        // Limpiar las suscripciones al destruir o desconectar
        if (!IsOwner) return;
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.RemovePriorityListener(OnCharTyped);
            InputHandler.Instance.OnInputModeChanged -= OnInputModeChanged;
        }
    }

    private void HandleCardPlayed(CardEventArgs args)
    {
        if (!IsSpawned || !IsOwner) return;

        // Acceder a la carta casteada
        if (!CardRegister.Instance.TryGetById(args.CardId, out CardDefinition cardDef)) return;

        string spellName = cardDef.Name;
        
        // Intentar ver si la carta estaba almacenada en la lista de cartas lanzadas con validación local
        // En caso afirmativo, se dispone de su texto exacto con posibles procesamientos
        if (locallyTypedSpellsCache.TryGetValue(args.CardId, out var queue) && queue.Count > 0)
        {
            spellName = queue.Dequeue();
        }
        int cardNameL = spellName.Length;

        // Buscar la última aparición exacta en el texto filtrado
        int anchorFiltered = filteredPlain.ToString().LastIndexOf(spellName, StringComparison.Ordinal);
        if (anchorFiltered != -1)
        {
            castSpell.Add(new CastSpell { Name = spellName, Start = anchorFiltered, Length = cardNameL });
            castSpell.Sort((a, b) => a.Start.CompareTo(b.Start));
        }
        else
        {
            Debug.LogWarning($"No se encontró '{spellName}' en el texto filtrado del chat.");
        }

        // Buscar la última aparición exacta en el texto raw
        int anchorRaw = ownerRaw.ToString().LastIndexOf(spellName, StringComparison.Ordinal);
        if (anchorRaw != -1)
        {
            castSpellRaw.Add(new CastSpell { Name = spellName, Start = anchorRaw, Length = cardNameL });
            castSpellRaw.Sort((a, b) => a.Start.CompareTo(b.Start));
        }
        else
        {
            Debug.LogWarning($"No se encontró '{spellName}' en el texto raw del chat.");
        }

        // Refrescar los textos visuales
        UpdateOutputs();
    }  

    private void OnInputModeChanged(InputModeMask mode)
    {
        // Detectar cambios de modo y preparar un posible espacio inicial
        bool wasCastingSpells = castingSpellMode;
        castingSpellMode = (mode == InputModeMask.Spells);
        if (castingSpellMode && !wasCastingSpells)
            pendingModeSpace = true;
    }

    private void OnCharTyped(char c)
    {
        // Ignorar la entrada si no es modo hechizo o si el carácter es inválido
        if (!castingSpellMode) return;

        if (char.IsControl(c) || c == marker.SpellMarker) return;

        // Evitar dobles espacios y aplicar el espacio pendiente
        if (c == ' ')
        {
            pendingModeSpace = false;
            InjectSpaceSafely();
            UpdateOutputs();
            return;
        }

        var pairs = GetActiveCardTcUIPairs();

        bool charAdvancesProgress = false;
        bool charStartsSpell = false;

        // Analizar cada typanble controller
        foreach (var (tc, cardUI) in pairs)
        {
            // Obtener el texto de la carte
            string cardTcText = tc.Text ?? "";
            if (cardTcText.Length == 0) continue;

            // La letra pulsada coincide con la letra de algún hechizo
            bool matchesExpected = tc.Idx < cardTcText.Length && cardTcText[tc.Idx] == c;

            if (matchesExpected)
            {
                if (tc.Idx == 0)
                {
                    charStartsSpell = true;
                }
                else
                {
                    // Está progresando en la escritura de un hechizo
                    charAdvancesProgress = true;
                }
            }
        }

        // Determinar si la tecla es útil y si inicia una nueva palabra
        bool useful = charAdvancesProgress || charStartsSpell;
        bool startedOtherNewSpell = charStartsSpell && !charAdvancesProgress;

        // Comprobar la necesidad de inyectar espacios automáticos
        bool isContinuingSpell = useful && !startedOtherNewSpell;

        if (pendingModeSpace && isContinuingSpell)
            pendingModeSpace = false;

        if (pendingModeSpace || startedOtherNewSpell)
        {
            InjectSpaceSafely();
            pendingModeSpace = false;
        }

        // Añadir el carácter al texto crudo y hacer trim
        ownerRaw.Append(c);
        TrimIfNeeded(ownerRaw, castSpellRaw);

        // Añadir el carácter al texto filtrado solo si es válido y hacer trim
        if (useful)
        {
            filteredPlain.Append(c);
            TrimIfNeeded(filteredPlain, castSpell);
        }

        // Update de los textos finales
        UpdateOutputs();
    }

    private void UpdateOutputs()
    {
        // Generar las cadenas coloreadas y sincronizar con la red.
        // El texto de procesamiento (filteredPlain, ownerRaw) pasan a formatearse
        // con los marcadores de color. Se activan los eventos y se actualizan las NV.
        CurrentLocalFilteredText = BuildFormattedOutput(filteredPlain.ToString(), castSpell);
        CurrentLocalRawText = BuildFormattedOutput(ownerRaw.ToString(), castSpellRaw);

        OnLocalFilteredTextChanged?.Invoke(CurrentLocalFilteredText);
        OnLocalRawTextChanged?.Invoke(CurrentLocalRawText);

        if (IsSpawned && IsOwner)
        {
            FilteredText.Value = ToFs512(CurrentLocalFilteredText);
            RawText.Value = ToFs512(CurrentLocalRawText);
        }
    }
    private void InjectSpaceSafely()
    {
        // Insertar un espacio en el raw si no hay uno ya
        if (ownerRaw.Length > 0 && ownerRaw[ownerRaw.Length - 1] != ' ')
        {
            ownerRaw.Append(' ');
            TrimIfNeeded(ownerRaw, castSpellRaw);
        }

        // Insertar un espacio en el texto filtrado si no hay uno ya
        if (filteredPlain.Length > 0 && filteredPlain[filteredPlain.Length - 1] != ' ')
        {
            filteredPlain.Append(' ');
            TrimIfNeeded(filteredPlain, castSpell);
        }
    }   

    private void TrimIfNeeded(StringBuilder buffer, List<CastSpell> castList)
    {
        // Salir si el texto no sobrepasa el límite
        if (buffer.Length <= MaxPlainChars) return;

        // Recortar el inicio del texto
        int trimCount = buffer.Length - MaxPlainChars;
        buffer.Remove(0, trimCount);

        // Ajustar las posiciones de los hechizos coloreados
        UpdateCastedSpellsPos(castList, trimCount);
    }

    private void UpdateCastedSpellsPos(List<CastSpell> castList, int trimCount)
    {
        // Recorrer del final al inicio
        for (int i = castList.Count - 1; i >= 0; i--)
        {
            CastSpell spell = castList[i];
            spell.Start -= trimCount;

            if (spell.Start < 0)
            {
                // El spell ya no es visible: borrar
                castList.RemoveAt(i);
            }
            else
            {
                // Actualizar la lista
                castList[i] = spell;
            }
        }
    }

    private string BuildFormattedOutput(string textToFormat, List<CastSpell> castList)
    {
        // Insertar el formato de color en las posiciones correspondientes de los hechizos
        if (castList.Count == 0) return textToFormat;

        var sb = new StringBuilder(textToFormat.Length + castList.Count * 4);
        int i = 0;

        foreach (var cast in castList)
        {
            if (cast.Start + cast.Length > textToFormat.Length) continue;
            if (cast.Start < i) continue;

            if (cast.Start > i)
                sb.Append(textToFormat, i, cast.Start - i);

            if (cast.Start > 0 && sb.Length > 0 && sb[sb.Length - 1] != ' ')
                sb.Append(' ');

            sb.Append(marker.SpellMarker)
              .Append(cast.Name)
              .Append(marker.SpellMarker)
              .Append(' ');

            i = cast.Start + cast.Length;

            if (i < textToFormat.Length && textToFormat[i] == ' ')
                i++;
        }

        if (i < textToFormat.Length)
            sb.Append(textToFormat, i, textToFormat.Length - i);

        return sb.ToString();
    }

    private List<(TypableController tc, CardUI ui)> GetActiveCardTcUIPairs()
    {
        // Obtener todos los cardUIs actuales
        currentCardUIs.Clear();
        foreach (var ui in GetComponentsInChildren<CardUI>(true))
        {
            if (ui.CardDefinition == null) continue;
            var tc = ui.GetComponentInChildren<TypableController>(true);
            if (tc != null)
                currentCardUIs.Add((tc, ui));
        }
        return currentCardUIs;
    }

    private FixedString512Bytes ToFs512(string s)
    {
        // Truncar la cadena para cumplir con el límite de bytes de Unity Netcode
        var fs = new FixedString512Bytes();
        if (string.IsNullOrEmpty(s)) return fs;

        const int CapBytes = 509;
        int byteCount = Encoding.UTF8.GetByteCount(s);

        while (byteCount > CapBytes && s.Length > 1)
        {
            s = s.Substring(1);
            byteCount = Encoding.UTF8.GetByteCount(s);
        }

        fs.CopyFromTruncated(s);
        return fs;
    }

    public override void OnDestroy()
    {
        if (deckController)
        {
            deckController.OnCardPlayedEvent -= HandleCardPlayed;
            deckController.OnLocalSpellExactTCText -= HandleLocalSpellTypedExact;
            deckController.OnCardPlayRequestFailed -= HandleCardPlayFailed;
        }

        base.OnDestroy();
    }
}