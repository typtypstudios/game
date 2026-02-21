using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    public float MaxValue { get; set; } = 1f;
    private Image bar;

    private void Awake()
    {
        bar = GetComponent<Image>();
    }

    public void UpdateValue(float oldValue, float newValue)
    {
        bar.fillAmount = newValue / MaxValue;
    }
}
