using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TypTyp;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstantMagnitudeIncreaseEffect", menuName = "TypTyp/Effects/ConstantMagnitudeIncreaseEffect")]
public class ConstantMagnitudeIncreaseEffect : StatusEffectDefinition
{
    [Range(-100, 100)][SerializeField] private float totalPercentage;
    [SerializeField] private float timeInterval;
    [SerializeField] private MagnitudeType type = MagnitudeType.Corruption;
    private float magnitudePerTick;
    private int millisecondsTime;
    private Dictionary<Player, CancellationTokenSource> cts = new();

    public override void OnActivate(Player target)
    {
        if (!target.IsServer) return;
        cts[target] = new();
        millisecondsTime = (int)(timeInterval * 1000);
        float maxValue = type == MagnitudeType.Corruption ?
            Settings.Instance.MaxCorruption : Settings.Instance.MaxMana;
        int numTicks = (int)Mathf.Ceil(DurationValue / timeInterval);
        magnitudePerTick = totalPercentage / 100 * maxValue;
        magnitudePerTick /= numTicks;
        _ = CorruptCoroutine(target, type, cts[target].Token);
    }

    public override void OnDeactivate(Player target)
    {
        if (!target.IsServer) return;
        cts[target].Cancel();
    }

    public override string GetDefaultValue()
    {
        return $"{Mathf.Abs(totalPercentage)}%";
    }

    private async Task CorruptCoroutine(Player target, MagnitudeType type, CancellationToken token)
    {
        bool isFirstTick = true;

        while (true)
        {
            if (token.IsCancellationRequested) return;
            if (type == MagnitudeType.Corruption)
                target.CorruptionManager.AddCorruption(magnitudePerTick, isPassive: !isFirstTick);
            else target.ManaManager.AddMana(magnitudePerTick);

            // A partir del segundo tick, el efecto se declara como pasivo y ya no se notifica
            // su sonido, sin afectar a ninguna otra cosa de su lógica.
            isFirstTick = false;

            await Task.Delay(millisecondsTime, token);
        }
    }
}

public enum MagnitudeType
{
    Corruption,
    Mana
}