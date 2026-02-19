using UnityEngine;

public class ManaSystem : MonoBehaviour
{
    public int CurrentMana { get; private set; }

    public void AddMana(int amount)
    {
        CurrentMana += amount;
    }

    public bool ConsumeMana(int amount)
    {
        if (CurrentMana < amount) return false;

        CurrentMana -= amount;
        return true;
    }
}
