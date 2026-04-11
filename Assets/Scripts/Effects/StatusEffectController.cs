using System.Collections.Generic;
using TMPro;
using TypTyp.TextSystem;
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
    public UnityEvent<StatusEffect> OnEffectApplied;
    public UnityEvent<StatusEffect> OnEffectRemoved;
    public UnityEvent<StatusEffect> OnEffectExpired;
    public UnityEvent<StatusEffect> OnEffectRefreshed;
    public List<StatusEffect> Effects => activeEffects;

    Player player;
    NetworkTextProvider textProvider;
    List<StatusEffect> toRemove;
    
    public void Awake()
    {
        player = GetComponent<Player>();
        textProvider = GetComponent<NetworkTextProvider>();
        textProvider.OnLineRequested += OnRitualLineRequested;
        activeEffects = new();
        toRemove = new();
    }

    private void OnDestroy() => textProvider.OnLineRequested -= OnRitualLineRequested;

    void Update()
    {
        HandleEffectExpiration(EffectDurationType.Time);
    }

    void OnRitualLineRequested()
    {
        HandleEffectExpiration(EffectDurationType.Lines);
    }

    public void AddEffect(StatusEffectDefinition effectDef)
    {
        var statusEffect = CreateStatusEffect(effectDef);

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
        statusEffect.Activate();
        OnEffectApplied?.Invoke(statusEffect);
    }

    void ExpireEffect(StatusEffect effect)
    {
        OnEffectExpired?.Invoke(effect);
        RemoveEffect(effect);
    }

    public void RemoveEffect(StatusEffect effect)
    {
        effect.Deactivate();
        activeEffects.Remove(effect);
        OnEffectRemoved?.Invoke(effect);
    }

    // Assume that refreshable effects are only those not immediate and added to the active effects list
    void RefreshEffect(StatusEffect effect)
    {
        effect.RemainingDuration = effect.Definition.DurationValue;
        OnEffectRefreshed.Invoke(effect);
    }

    StatusEffect CreateStatusEffect(StatusEffectDefinition definition)
    {
        return new StatusEffect(definition, player);
    }

    private void HandleEffectExpiration(EffectDurationType durationType)
    {
        foreach (var effect in activeEffects)
        {
            if (effect.Definition.DurationType == durationType)
            {
                effect.RemainingDuration -= durationType == EffectDurationType.Time ? Time.deltaTime : 1;

                if (effect.RemainingDuration <= 0)
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
