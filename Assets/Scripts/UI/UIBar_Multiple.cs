using UnityEngine;
using System;
using System.Linq;

public class UIBar_Multiple : MonoBehaviour, IFillableBar
{
    protected IFillableBar[] bars;
    public float MaxValue { get; set; } = 1f;
    public event Action<float> OnValueUpdated;

    private void Awake()
    {
        bars = GetComponentsInChildren<IFillableBar>().Where(f => f != this).ToArray();
    }

    public virtual void UpdateValue(float oldValue, float newValue)
    {
        StopAllCoroutines();
        float normalizedValue = newValue / MaxValue;
        foreach(var bar in bars) bar.UpdateValue(oldValue, normalizedValue);
        OnValueUpdated?.Invoke(newValue);
    }

    public virtual void SetValueWithoutTransition(float value)
    {
        StopAllCoroutines();
        float normalizedValue = value / MaxValue;
        foreach (var bar in bars) bar.SetValueWithoutTransition(normalizedValue);
        OnValueUpdated?.Invoke(value);
    }
}
