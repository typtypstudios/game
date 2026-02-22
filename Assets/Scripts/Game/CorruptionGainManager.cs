using System;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class CorruptionGainManager : MonoBehaviour
{
    private Player player;
    public event Action<float> OnCorruptionGain;
    public float GainMultiplier { get; set; } = 1;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    public void ProcessMistake()
    {
        AddCorruption(Settings.MistakePenalizationPercentage / 100 * Settings.MaxCorruption);
    }

    private void AddCorruption(float corruptionToAdd)
    {
        float currentCorruption = player.CurrentCorruption.Value;
        currentCorruption = Mathf.Clamp(currentCorruption + corruptionToAdd, 0, Settings.MaxCorruption);
        OnCorruptionGain?.Invoke(currentCorruption);
    }
}
