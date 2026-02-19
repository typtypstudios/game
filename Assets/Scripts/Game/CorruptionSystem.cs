using UnityEngine;

public class CorruptionSystem : MonoBehaviour
{
    public int CurrentCorruption { get; private set; }

    public void AddCorruption(int amount)
    {
        CurrentCorruption += amount;
    }
}
