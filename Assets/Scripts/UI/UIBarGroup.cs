using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBarGroup : MonoBehaviour
{
    protected List<IFillableBar> bars = new();
    protected float perBarPercentage;
    public float MaxValue { get; set; } = 1f;

    void Start()
    {
        if (bars.Count == 0) //A lo mejor se ha creado en un componente hijo y ya está el array completo
            bars = GetComponentsInChildren<IFillableBar>().ToList();
        perBarPercentage = 1f / bars.Count;
    }

    public virtual void UpdateValue(float oldValue, float newValue)
    {
        float normalizedValue = newValue / MaxValue;
        foreach (IFillableBar bar in bars)
        {
            float barValue = normalizedValue >= perBarPercentage ? 1.0f : normalizedValue / perBarPercentage;
            bar.UpdateValue(0, barValue);
            normalizedValue = Mathf.Clamp01(normalizedValue - perBarPercentage);
        }
    }
}
