using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Este componente se encarga de manejar los efectos de estado que tiene un jugador. 
/// Permite agregar, remover y actualizar efectos de estado, y se asegura de que los efectos se apliquen correctamente al jugador objetivo.
/// </summary>
[RequireComponent(typeof(Player))]
public class StatusEffectController : MonoBehaviour
{
    [SerializeField] List<StatusEffect> activeEffects;
    public UnityEvent<StatusEffect> OnEffectApplied = new();
    public UnityEvent<StatusEffect> OnEffectRemoved = new();
    public UnityEvent<StatusEffect> OnEffectExpired = new();
    public UnityEvent<StatusEffect> OnEffectRefreshed = new();
    public List<StatusEffect> Effects => activeEffects;

    Player player;
    List<StatusEffect> toRemove;
    private System.Random random;

    void Awake()
    {
        player = GetComponent<Player>();
        activeEffects = new();
        toRemove = new();
    }

    void Start()
    {
        player.RitualManager.OnLineCompleted += OnRitualLineCompleted;
    }

    void OnDestroy()
    {
        player.RitualManager.OnLineCompleted -= OnRitualLineCompleted;
    }

    void Update()
    {
        HandleEffectExpiration(EffectDurationType.Time, 0);
    }

    void OnRitualLineCompleted(int completedLines)
    {
        HandleEffectExpiration(EffectDurationType.Lines, completedLines);
    }

    public void AddEffect(StatusEffectDefinition effectDef)
    {
        if (random == null)
        {
            Debug.LogError("StatusEffectController random not configured. Call SetRandom before AddEffect.");
            return;
        }

        var statusEffect = CreateStatusEffect(effectDef);
        ConfigureLineDuration(statusEffect);

        // Refresh
        var refreshMatch = activeEffects.Find(e => e.Equals(statusEffect));
        if (refreshMatch != default)
        {
            RefreshEffect(refreshMatch);
            return;
        }

        // Polarity
        var oppositeMatch = activeEffects.Find(e => e.Definition.IsOpposite(effectDef));
        if (oppositeMatch != default)
        {
            RemoveEffect(oppositeMatch);
            return;
        }

        // Addition and activation
        if (effectDef.DurationType != EffectDurationType.Immediate &&
            effectDef.DurationType != EffectDurationType.Permanent)
            activeEffects.Add(statusEffect);
        statusEffect.Activate(new EffectContext(player, random));
        OnEffectApplied?.Invoke(statusEffect);
    }

    void ExpireEffect(StatusEffect effect)
    {
        OnEffectExpired?.Invoke(effect);
        RemoveEffect(effect);
    }

    public void RemoveEffect(StatusEffect effect)
    {
        if (random == null)
        {
            Debug.LogError("StatusEffectController random not configured. Call SetRandom before RemoveEffect.");
            return;
        }

        effect.Deactivate(new EffectContext(player, random));
        activeEffects.Remove(effect);
        OnEffectRemoved?.Invoke(effect);
    }

    public void SetRandom(System.Random random)
    {
        this.random = random ?? throw new System.ArgumentNullException(nameof(random));
    }

    // Assume that refreshable effects are only those not immediate and added to the active effects list
    void RefreshEffect(StatusEffect effect)
    {
        effect.RemainingDuration = effect.Definition.DurationValue;
        ConfigureLineDuration(effect);
        OnEffectRefreshed.Invoke(effect);
    }

    StatusEffect CreateStatusEffect(StatusEffectDefinition definition)
    {
        return new StatusEffect(definition, player);
    }

    private void ConfigureLineDuration(StatusEffect effect)
    {
        if (effect.Definition.DurationType != EffectDurationType.Lines)
            return;

        int firstAffectedLine = player.RitualManager.CurrentLineIndex + 1;
        int lineCount = Mathf.CeilToInt(effect.Definition.DurationValue);
        effect.SetAffectedLineWindow(firstAffectedLine, lineCount);
    }

    private void HandleEffectExpiration(EffectDurationType durationType, int completedLines)
    {
        foreach (var effect in activeEffects)
        {
            if (effect.Definition.DurationType == durationType)
            {
                if (durationType == EffectDurationType.Time)
                {
                    effect.RemainingDuration -= Time.deltaTime;
                }
                else
                {
                    effect.RemainingDuration = Mathf.Max(0, effect.LastAffectedLineIndex - completedLines + 1);
                }

                bool isExpired = durationType == EffectDurationType.Lines
                    ? completedLines > effect.LastAffectedLineIndex
                    : effect.RemainingDuration <= 0;

                if (isExpired)
                {
                    toRemove.Add(effect);
                }
            }
        }

        foreach (var effect in toRemove)
        {
            ExpireEffect(effect);
        }

        toRemove.Clear();
    }
}
