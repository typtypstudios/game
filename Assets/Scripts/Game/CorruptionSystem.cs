using UnityEngine;

public class CorruptionSystem : MonoBehaviour
{
    public float CurrentCorruption { get; private set; }

    // Variables
    private float corruptionUnit = 1.0f;
    private float multiplier = 1.0f;
    private float maxCorruption = 5.0f;

    private GameUIConfigurator uiManager;

    private void Awake()
    {
        FindFirstObjectByType<RitualManager>().OnWrongChar += AddCorruption;
        uiManager = FindFirstObjectByType<GameUIConfigurator>();
    }

    public void AddCorruption()
    {
        CurrentCorruption += corruptionUnit * multiplier;
        if(CurrentCorruption >= maxCorruption)
        {
            Debug.LogError("Loss condition not implemented");
            CurrentCorruption = maxCorruption;
        }
        //uiManager.SetCorruptionProgress(CurrentCorruption / maxCorruption);
    }
}
