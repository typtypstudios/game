using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UIBar : MonoBehaviour, IFillableBar
{
    [SerializeField] protected Image filler;
    [SerializeField] protected float updateTime = 0.5f;
    [SerializeField] private FillType fillType;
    protected Image bar;
    public float MaxValue { get; set; } = 1f;
    public event Action<float> OnValueUpdated;
    private float FillAmount
    {
        get 
        {
            if (fillType == FillType.Fill) return filler.fillAmount;
            else return filler.color.a;
        }
        set
        {
            if(fillType == FillType.Fill) filler.fillAmount = value;
            else filler.color = new(filler.color.r, filler.color.g, filler.color.b, value);
        }
    }

    private void Awake()
    {
        bar = GetComponent<Image>();
    }

    public virtual void UpdateValue(float oldValue, float newValue)
    {
        StopAllCoroutines();
        float normalizedValue = newValue / MaxValue;
        StartCoroutine(UpdateBarCorroutine(normalizedValue));
        OnValueUpdated?.Invoke(normalizedValue);
    }

    public virtual void SetValueWithoutTransition(float value)
    {
        StopAllCoroutines();
        bar.fillAmount = value;
        filler.fillAmount = value;
        OnValueUpdated?.Invoke(value);
    }

    IEnumerator UpdateBarCorroutine(float target)
    {
        float speed = (target - FillAmount) / updateTime;
        bar.fillAmount = target;
        while (FillAmount < target)
        {
            FillAmount = Mathf.MoveTowards(FillAmount, target, speed * Time.deltaTime);
            yield return null;
        }
        FillAmount = target;
    }

    private enum FillType
    {
        Fill,
        Alpha
    }
}
