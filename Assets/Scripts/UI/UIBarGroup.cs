using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBarGroup : MonoBehaviour
{
    [Header("Manual creation:")]
    [SerializeField] private WeightedBar[] weightedBars;
    protected List<IFillableBar> bars = new();
    protected float perBarPercentage;
    public float MaxValue { get; set; } = 1f;
    private bool UsingWeights => weightedBars.Length > 0;

    void Start()
    {
        if (UsingWeights) NormalizeWeights();
        else InitDefault();
    }

    private void NormalizeWeights()
    {
        float sum = 0;
        foreach (var bar in weightedBars)
            sum += bar.weight;
        foreach (var bar in weightedBars)
            bar.weight /= sum;
    }

    private void InitDefault()
    {
        if (bars.Count == 0) //A lo mejor se ha creado en un componente hijo y ya está el array completo
            bars = GetComponentsInChildren<IFillableBar>(true).
                Where(b => {
                    var c = b as Component; return c != null &&
                    c.transform.parent != null &&
                    (c.transform.GetComponent<UIBar_Multiple>() != null ||
                    c.transform.parent.GetComponentInParent<UIBar_Multiple>() == null);
                })
                .ToList();
        perBarPercentage = 1f / bars.Count;
    }

    public virtual void UpdateValue(float oldValue, float newValue)
    {
        if (!UsingWeights) DefaultUpdate(newValue);
        else WeightedUpdate(newValue);
    }

    private void DefaultUpdate(float newValue)
    {
        float normalizedValue = newValue / MaxValue;
        foreach (IFillableBar bar in bars)
        {
            float barValue = normalizedValue >= perBarPercentage ? 1.0f : normalizedValue / perBarPercentage;
            bar.UpdateValue(0, barValue);
            normalizedValue = Mathf.Clamp01(normalizedValue - perBarPercentage);
        }
    }

    private void WeightedUpdate(float newValue)
    {
        float normalizedValue = newValue / MaxValue;
        foreach (var wbar in weightedBars)
        {
            float barValue = normalizedValue >= wbar.weight ? 1.0f : normalizedValue / wbar.weight;
            wbar.bar.UpdateValue(0, barValue);
            normalizedValue = Mathf.Clamp01(normalizedValue - wbar.weight);
        }
    }
}

[Serializable]
public class WeightedBar
{
    public UIBar bar;
    [Range(0, 1)] public float weight = 1;
}