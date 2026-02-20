using UnityEngine;

public class CorruptionSystem : MonoBehaviour
{
    public float CurrentCorruption { get; private set; }

    // Variables
    private float corruptionUnit = 1;
    private float multiplier;
    private float maxCorruption = 5;

    private UIManager uiManager;

    private void Awake()
    {
        FindFirstObjectByType<RitualManager>().OnErrorChar += AddCorruption;
        uiManager = FindFirstObjectByType<UIManager>();
    }

    private void Start()
    {
        multiplier = 1.0f;
    }

    public void AddCorruption()
    {
        CurrentCorruption += corruptionUnit * multiplier;
        if(CurrentCorruption >= maxCorruption)
        {
            Debug.LogError("Loss condition not implemented");
            CurrentCorruption = maxCorruption;
        }
        uiManager.SetCorruptionProgress(CurrentCorruption / maxCorruption);
    }
}
