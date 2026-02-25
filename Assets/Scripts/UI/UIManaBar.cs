using TypTyp;
using UnityEngine;
using System.Collections.Generic;

public class UIManaBar : MonoBehaviour
{
    [Tooltip("Separación entre barras expresado en porcentaje del width de mana bar")]
    [Range(0f, 0.1f)][SerializeField] private float barSeparation = 0.01f;
    [SerializeField] private GameObject manaBarPrefab;
    private List<UIBar> bars = new();
    private float perBarPercentage;
    public float MaxValue { get; set; } = 1f;

    private void Awake()
    {
        CreateBars();
        perBarPercentage = 1f / bars.Count;
    }

    public void UpdateValue(float oldValue, float newValue)
    {
        float normalizedValue = newValue / MaxValue;
        foreach(UIBar bar in bars)
        {
            float barValue = normalizedValue >= perBarPercentage ? 1.0f : normalizedValue / perBarPercentage;
            bar.UpdateValue(0, barValue);
            normalizedValue = Mathf.Clamp01(normalizedValue - perBarPercentage);
        }
    }

    private void CreateBars()
    {
        float totalWidth = GetComponent<RectTransform>().sizeDelta.x;
        float totalSeparation = (Settings.Instance.NumManaBars - 1) * barSeparation * totalWidth;
        float barWidth = (totalWidth - totalSeparation) / Settings.Instance.NumManaBars;
        for(int i = 0; i < Settings.Instance.NumManaBars; i++)
        {
            RectTransform bar = Instantiate(manaBarPrefab, transform).GetComponent<RectTransform>();
            bar.sizeDelta = new Vector2(barWidth, bar.sizeDelta.y);
            bars.Add(bar.GetComponent<UIBar>());
        }
    }
}
