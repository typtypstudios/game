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
    public UnityEvent<StatusEffect> OnEffectExpired; //No se si es distinto a Removed
    public UnityEvent<StatusEffect> OnEffectRefreshed;

    Player player;
    NetworkTextProvider textProvider;
    List<StatusEffect> toRemove;
    public List<TMP_FontAsset> ActiveFonts { get; private set; } = new(); //Fuentes provenientes de modificadores

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

    public void AddEffect(StatusEffectDefinition effectDef)
    {
        var statusEffect = CreateStatusEffect(effectDef);

        //Refresh
        var refreshMatch = activeEffects.Find(e => e.Equals(statusEffect));
        if (refreshMatch != default)
        {
            RefreshEffect(refreshMatch);
            return;
        }

        //Polarity
        var oppositeMatch = activeEffects.Find(e => e.Definition.IsOpposite(effectDef));
        if (oppositeMatch != default)
        {
            RemoveEffect(oppositeMatch);
        }

        //Addition and activation
        if (effectDef.DurationType != EffectDurationType.Immediate) activeEffects.Add(statusEffect);
        statusEffect.Activate();
        OnEffectApplied?.Invoke(statusEffect);
    }

    void ExpireEffect(StatusEffect effect)
    {
        OnEffectExpired?.Invoke(effect);
        RemoveEffect(effect);
    }

    void RemoveEffect(StatusEffect effect)
    {
        effect.Deactivate();
        //if (effect.Definition.DurationType != EffectDurationType.Immediate)
        activeEffects.Remove(effect);
        OnEffectRemoved?.Invoke(effect);
    }

    //Asumo que los efectos que se pueden refrescar
    //solo son aquellos no inmediatos que entran a la coleccion de efectos
    void RefreshEffect(StatusEffect effect)
    {
        effect.RemainingDuration = effect.Definition.DurationValue;
        OnEffectRefreshed.Invoke(effect);
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