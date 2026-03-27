using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using TypTyp;

[CreateAssetMenu(fileName = "ConstantCorruptionEffect", menuName = "TypTyp/Effects/ConstantCorruptionEffect")]
public class ConstantCorruptionEffect : StatusEffectDefinition
{
    [Range(-100, 100)][SerializeField] private float totalCorruptionPercentage;
    [SerializeField] private float timeInterval;
    private float corruptionPerTick;
    private int millisecondsTime;
    private CancellationTokenSource cts;

    public override void OnActivate(Player target)
    {
        if (!target.IsServer) return;
        cts = new();
        millisecondsTime = (int)(timeInterval * 1000);
        corruptionPerTick = totalCorruptionPercentage / 100 * Settings.Instance.MaxCorruption;
        int numTicks = (int)Mathf.Ceil(DurationValue / timeInterval);
        corruptionPerTick /= numTicks;
        _ = CorruptCoroutine(target.CorruptionManager, cts.Token);
    }

    public override void OnDeactivate(Player target)
    {
        if (!target.IsServer) return;
        cts.Cancel();
    }

    public override string GetDefaultValue()
    {
        return $"{Mathf.Abs(totalCorruptionPercentage)}%";
    }

    private async Task CorruptCoroutine(CorruptionGainManager manager, CancellationToken token)
    {
        while (true)
        {
            if (token.IsCancellationRequested) return;
            manager.AddCorruption(corruptionPerTick);
            await Task.Delay(millisecondsTime, token);
        }
    }
}