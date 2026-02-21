using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    private Image bar;

    private void Awake()
    {
        bar = GetComponent<Image>();
    }

    public void UpdateValue(float oldValue, float newValue)
    {
        bar.fillAmount = newValue;
    }
}
