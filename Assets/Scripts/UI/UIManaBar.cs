using TypTyp;
using UnityEngine;
using System.Collections.Generic;

public class UIManaBar : MonoBehaviour
{
    [Tooltip("Separación entre barras expresado en porcentaje del width de mana bar")]
    [Range(0f, 0.1f)][SerializeField] private float barSeparation = 0.01f;
    [SerializeField] private GameObject manaBarPrefab;
    private readonly List<UIBar> bars = new();
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
            if (newValue > oldValue) bar.UpdateValue(0, barValue);
            else bar.SetValueWithoutTransition(barValue);
                normalizedValue = Mathf.Clamp01(normalizedValue - perBarPercentage);
        }
    }

    private void CreateBars() //Se colocan las barras manualmente y no en un layout group porque va mejor
    {
        float totalWidth = GetComponent<RectTransform>().sizeDelta.x;
        float totalSeparation = (Settings.Instance.NumManaBars - 1) * barSeparation * totalWidth;
        float barWidth = (totalWidth - totalSeparation) / Settings.Instance.NumManaBars;
        float currentPos = barWidth / 2; //Posición de la primera barra
        for(int i = 0; i < Settings.Instance.NumManaBars; i++)
        {
            RectTransform bar = Instantiate(manaBarPrefab, transform).GetComponent<RectTransform>();
            bar.sizeDelta = new Vector2(barWidth, bar.sizeDelta.y);
            bar.anchoredPosition = new Vector2(currentPos, 0);
            currentPos += barWidth + barSeparation * totalWidth;
            UIBar uiBar = bar.GetComponentInChildren<UIBar>();
            if (i != 0) uiBar.PrevBar = bars[i - 1];
            bars.Add(uiBar);
        }
    }
}
