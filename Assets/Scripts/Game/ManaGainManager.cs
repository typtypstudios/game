using System;
using UnityEngine;

public class ManaGainManager : MonoBehaviour
{
    [Tooltip("Cuánto maná gana en relación con el progreso del ritual " +
        "(multiplica el progreso realizado")]
    [SerializeField] private float manaGain = 5; 
    [SerializeField] private float maxMana = 1;
    private Player player;
    public event Action<float> OnManaGain;
    public float GainMultiplier { get; set; } = 1;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        player.RitualProgress.OnValueChanged += AddMana;
    }

    private void OnDisable()
    {
        player.RitualProgress.OnValueChanged -= AddMana;
    }

    public void AddMana(float oldValue, float newValue)
    {
        float currentMana = player.CurrentMana.Value;
        currentMana += (newValue - oldValue) * manaGain * GainMultiplier;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        OnManaGain?.Invoke(currentMana);
    }

    /// <summary>
    /// Consumes mana
    /// </summary>
    /// <param name="amount">Quantity of mana to spend</param>
    /// <returns>True if there's enough mana, otherwise false</returns>
    //public bool ConsumeMana(int amount)
    //{
    //    //Debug.LogWarning("Esto debería ser un rpc");
    //    //if (CurrentMana < amount) return false;

    //    //CurrentMana -= amount;
    //    return true;
    //}
}
