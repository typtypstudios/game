using System;
using TypTyp;
using UnityEngine;

[NoAutoCreate]
[CreateAssetMenu(fileName = nameof(XPManager), menuName = "TypTyp/XPManager")]
public class XPManager : ScriptableSingleton<XPManager>
{
    [field: SerializeField] public float XPPerRank { get; set; } = 100f;
    [field: SerializeField] public Vector2 XPGainRange { get; set; } = new(15, 20); //Porcentaje!!
    [field: SerializeField] public float PerformanceMult { get; set; } = 1.2f;
    public event Action<float, float> OnXPUpdated;

    public void ProcessVictory()
    {
        float xpGain = Utils.RandomInRange(XPGainRange) / 100 *
            XPPerRank;
        float performanceDiff = Player.User.RitualProgress.Value - Player.Enemy.RitualProgress.Value;
        float xpMult = Mathf.Lerp(1, PerformanceMult, performanceDiff);
        float normalizedValue = xpGain * xpMult / XPPerRank;
        SaveState currentState = SaveManager.Instance.GetState();
        float prevXP = currentState.slot.cultData[currentState.slot.cultId].level;
        float newXP = prevXP + normalizedValue;
        currentState.slot.cultData[currentState.slot.cultId].level = 
            Mathf.Min(newXP, RuntimeVariables.Instance.MaxLevel);
        currentState.slot.profile.gamesWon += 1;
        SaveManager.Instance.Save();
        OnXPUpdated?.Invoke(prevXP, newXP);
    }

    public void ProcessLoss()
    {
        SaveState currentState = SaveManager.Instance.GetState();
        float prevXP = currentState.slot.cultData[currentState.slot.cultId].level;
        OnXPUpdated?.Invoke(prevXP, prevXP);
    }
}
