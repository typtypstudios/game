using UnityEngine;

public class RitualManager : MonoBehaviour
{
    private string ritualText;
    private int currentIndex;

    public void GenerateRitual(int seed)
    {
        Random.InitState(seed);
        ritualText = "loremipsumritual"; // Placeholder
        currentIndex = 0;
    }

    public bool Validate(char character)
    {
        if (currentIndex >= ritualText.Length) return false;

        if (ritualText[currentIndex] == character)
        {
            currentIndex++;
            return true;
        }

        return false;
    }

    public bool IsCompleted()
    {
        return currentIndex >= ritualText.Length;
    }
}
