using System;
using TypTyp;
using UnityEngine;

[NoAutoCreate]
[CreateAssetMenu(fileName = nameof(XPManager), menuName = "TypTyp/XPManager")]
public class XPManager : ScriptableSingleton<XPManager>
{
    public event Action<float, float> OnXPUpdated;

    public void ProcessVictory()
    {
        float xpGain = Utils.RandomInRange(Settings.Instance.XPGainRange) / 100 *
            Settings.Instance.XPPerRank;
        float performanceDiff = Player.User.RitualProgress.Value - Player.Enemy.RitualProgress.Value;
        float xpMult = Mathf.Lerp(1, Settings.Instance.PerformanceMult, performanceDiff);
        float normalizedValue = xpGain * xpMult / Settings.Instance.XPPerRank;
        SaveState currentState = SaveManager.Instance.GetState();
        float prevXP = currentState.slot.cultData[currentState.slot.cultId].level;
        float newXP = prevXP + normalizedValue;
        currentState.slot.cultData[currentState.slot.cultId].level = newXP;
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
