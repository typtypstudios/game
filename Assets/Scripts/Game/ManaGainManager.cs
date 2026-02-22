using System;
using UnityEngine;
using TypTyp;
[RequireComponent(typeof(Player))]
public class ManaGainManager : MonoBehaviour
{
    private Player player;
    public event Action<float> OnManaGain;
    public float GainMultiplier { get; set; } = 1;

    private void Start()
    {
        player = GetComponent<Player>();
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
    /// <param name="amount">Quantity of mana to spend</param>
    /// <returns>True if there's enough mana, otherwise false</returns>
    //public bool ConsumeMana(int amount)
    //{
    //    //Debug.LogWarning("Esto deber�a ser un rpc");
    //    //if (CurrentMana < amount) return false;

    //    //CurrentMana -= amount;
    //    return true;
    //}
}
