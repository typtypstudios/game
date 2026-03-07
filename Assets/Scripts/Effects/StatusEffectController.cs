using System.Collections.Generic;
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
    public UnityEvent<StatusEffect> OnEffectExpired; //No se si es distinto a Removed
    public UnityEvent<StatusEffect> OnEffectRefreshed;

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
        toRemove.Clear();

        for (int i = 0; i < activeEffects.Count; i++)
        {
            var effect = activeEffects[i];

            if (effect.Definition.DurationType == EffectDurationType.Time)
            {
                effect.RemainingDuration -= Time.deltaTime;

                if (effect.RemainingDuration <= 0)
                {
                    toRemove.Add(effect);
                }
            }
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            ExpireEffect(toRemove[i]);
        }
    }

    public void AddEffect(StatusEffectDefinition effect)
    {
        var statusEffect = CreateStatusEffect(effect);
        if (activeEffects.Contains(statusEffect))
        {
            //Se podria hacer refresco de efectos o sumar duracion
            // o stackeo en caso de reaplicar efectos
            return;
        }
        if(effect.DurationType != EffectDurationType.Immediate) activeEffects.Add(statusEffect);
        statusEffect.Activate();
        OnEffectApplied?.Invoke(statusEffect);
    }

    void ExpireEffect(StatusEffect effect)
    {
        effect.Deactivate();
        if (effect.Definition.DurationType != EffectDurationType.Immediate) activeEffects.Remove(effect);
        OnEffectExpired?.Invoke(effect);
        OnEffectRemoved?.Invoke(effect);
    }
    StatusEffect CreateStatusEffect(StatusEffectDefinition definition)
    {
        return new StatusEffect(definition, player);
    }

    void OnRitualLineRequested()
    {
        toRemove.Clear();

        for (int i = 0; i < activeEffects.Count; i++)
        {
            var effect = activeEffects[i];

            if (effect.Definition.DurationType == EffectDurationType.Lines)
            {
                effect.RemainingDuration -= 1;

                if (effect.RemainingDuration <= 0)
                {
                    toRemove.Add(effect);
                }
            }
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            ExpireEffect(toRemove[i]);
        }
    }
}