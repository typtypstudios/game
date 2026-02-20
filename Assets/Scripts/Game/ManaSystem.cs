using UnityEngine;

public class ManaSystem : MonoBehaviour
{
    public float CurrentMana { get; private set; }

    // Variables
    private float manaUnit = 0.1f;
    private float multiplier;
    private float maxMana = 10;

    private UIManager uiManager;

    private void Awake()
    {
        FindFirstObjectByType<RitualManager>().OnCorrectChar += AddMana;
        uiManager = FindFirstObjectByType<UIManager>();
    }

    private void Start()
    {
        multiplier = 1.0f;
    }

    public void AddMana()
    {
        CurrentMana += manaUnit * multiplier;
        if(CurrentMana > maxMana) CurrentMana = maxMana;

        uiManager.SetManaProgress(CurrentMana / maxMana);
    }

    /// <summary>
    /// Consumes mana
    /// </summary>
    /// <param name="amount">Quantity of mana to spend</param>
    /// <returns>True if there's enough mana, otherwise false</returns>
    public bool ConsumeMana(int amount)
    {
        Debug.LogWarning("Esto debería ser un tcp");
        if (CurrentMana < amount) return false;

        CurrentMana -= amount;
        uiManager.SetManaProgress(CurrentMana / maxMana);
        return true;
    }
}
