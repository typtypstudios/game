using UnityEngine;
using UnityEngine.UI;

public class SliderHelper : MonoBehaviour
{
    public Slider slider;

    public void IncreaseBy(float amount)
    {
        SetValue(slider.value + amount);
    }

    public void DecreaseBy(float amount)
    {
        SetValue(slider.value - amount);
    }

    public void SetValue(float newValue)
    {
        slider.value = Mathf.Clamp(newValue, slider.minValue, slider.maxValue);
    }

    public void SetValueWithoutNotify(float newValue)
    {
        slider.SetValueWithoutNotify(
            Mathf.Clamp(newValue, slider.minValue, slider.maxValue)
        );
    }

    public void IncreaseNormalized(float amount)
    {
        SetNormalized(slider.normalizedValue + amount);
    }

    public void DecreaseNormalized(float amount)
    {
        SetNormalized(slider.normalizedValue - amount);
    }

    public void SetNormalized(float normalized)
    {
        slider.normalizedValue = Mathf.Clamp01(normalized);
    }

    public void SetNormalizedWithoutNotify(float normalized)
    {
        slider.SetValueWithoutNotify(
            Mathf.Lerp(slider.minValue, slider.maxValue, Mathf.Clamp01(normalized))
        );
    }
}