using TMPro;
using UnityEngine;

public class RitualBar : UIBarGroup
{
    [SerializeField] private TMP_Text percentajeText;

    public override void UpdateValue(float oldValue, float newValue)
    {
        base.UpdateValue(oldValue, newValue);
        int percentaje = Mathf.FloorToInt(newValue * 100);
        percentajeText.text = $"{percentaje}%";
    }
}
