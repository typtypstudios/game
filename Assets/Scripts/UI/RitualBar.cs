using System.Collections;
using TMPro;
using UnityEngine;

public class RitualBar : UIBar
{
    [SerializeField] private TMP_Text percentajeText;

    public override void UpdateValue(float oldValue, float newValue)
    {
        base.UpdateValue(oldValue, newValue);
        int percentaje = Mathf.RoundToInt(newValue * 100);
        percentajeText.text = $"{percentaje}%";
    }

    public override void SetValueWithoutTransition(float value)
    {
        base.SetValueWithoutTransition(value);
        int percentaje = Mathf.FloorToInt(value * 100);
        percentajeText.text = $"{percentaje}%";
    }
}
