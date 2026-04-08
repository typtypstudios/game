using System;
using UnityEngine;
using TypTyp;
using System.Collections;

[RequireComponent(typeof(Player))]
public class CorruptionGainManager : MonoBehaviour
{
    private Player player;
    public event Action<float> OnCorruptionGain;
    private WaitForSeconds timeToAutoHeal;
    private WaitForSeconds autoHealInterval;
    public event Action<float, Player> OnMistake;
    public float HurtGainMultiplier { get; set; } = 1;
    public float HealGainMultiplier { get; set; } = 1;
    public float MistakeGainMultiplier { get; set; } = 1;

    private void Awake()
    {
        player = GetComponent<Player>();
        timeToAutoHeal = new WaitForSeconds(Settings.Instance.TimeToAutoHeal);
        autoHealInterval = new WaitForSeconds(Settings.Instance.AutoHealInterval);
    }
    
    public void ProcessMistake()
    {
        Debug.Log("Processing mistake for player " + player.name, gameObject);
        float corruption = Settings.Instance.MistakePenalizationPercentage / 100 *
            Settings.Instance.MaxCorruption * MistakeGainMultiplier;
        AddCorruption(corruption);
        OnMistake?.Invoke(corruption, player);
    }

    public void AddCorruption(float corruptionToAdd)
    {
        if(corruptionToAdd > 0)
        {
            StopAllCoroutines();
            StartCoroutine(AutoHealCoroutine());
        }
        float multiplier = corruptionToAdd > 0 ? HurtGainMultiplier : HealGainMultiplier;
        corruptionToAdd *= multiplier;
        float currentCorruption = player.CurrentCorruption.Value;
        currentCorruption = Mathf.Clamp(currentCorruption + corruptionToAdd, 0, 
            Settings.Instance.MaxCorruption);
        OnCorruptionGain?.Invoke(currentCorruption);
    }

    IEnumerator AutoHealCoroutine()
    {
        yield return timeToAutoHeal;
        while (true)
        {
            AddCorruption(-Settings.Instance.AutoHealPercentage / 100 * Settings.Instance.MaxCorruption);
            yield return autoHealInterval;
        }
    }
}
