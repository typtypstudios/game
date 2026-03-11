using System;
using UnityEngine;
using TypTyp;

[RequireComponent(typeof(Player))]
public class ManaGainManager : MonoBehaviour
{
    private Player player;
    public event Action<float> OnManaGain;
    public event Action<int> OnCostModifierChangedEvent;

    public float GainMultiplier { get; set; } = 1;
    public int CostModifier { get; private set; } = 0;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void OnEnable()
    {
        player.RitualProgress.OnValueChanged += AddMana;
    }

    private void OnDisable()
    {
        player.RitualProgress.OnValueChanged -= AddMana;
    }

    public void AddMana(float oldValue, float newValue)
    {
        float currentMana = player.CurrentMana.Value;
        currentMana += (newValue - oldValue) * Settings.Instance.MaxMana * Settings.Instance.ManaGain * GainMultiplier;
        currentMana = Mathf.Clamp(currentMana, 0, Settings.Instance.MaxMana);
        OnManaGain?.Invoke(currentMana);
    }

    /// <summary>
    /// Consumes mana
    /// </summary>
    /// <param name="barAmount">Quantity of mana to spend</param>
    /// <returns>True if there's enough mana, otherwise false</returns>
    public bool ConsumeMana(int barAmount)
    {
        float currentMana = player.CurrentMana.Value;
        currentMana -= GetTotalCost(barAmount);
        currentMana = Mathf.Clamp(currentMana, 0, Settings.Instance.MaxMana);
        OnManaGain?.Invoke(currentMana);
        return true;
    }

    public void AddCostModifier(int modifier)
    {
        CostModifier += modifier;
        OnCostModifierChangedEvent?.Invoke(CostModifier);
    }

    public float GetTotalCost(float baseAmount)
    {
        return baseAmount + CostModifier;
    }

    public bool CanAfford(float baseAmount)
    {
        return baseAmount + CostModifier <= player.CurrentMana.Value;
    }
}
