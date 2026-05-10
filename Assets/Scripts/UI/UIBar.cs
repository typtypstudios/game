using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UIBar : MonoBehaviour, IFillableBar
{
    [SerializeField] protected Image filler;
    [SerializeField] private FillType fillType;
    [Min(0.01f)][SerializeField] protected float updateTime = 0.5f;
    [SerializeField] private bool replaceParentWithFill;
    protected Image bar;
    public float MaxValue { get; set; } = 1f;
    public event Action<float> OnValueUpdated;
    private float FillerAmount
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
            if (replaceParentWithFill) BackgroundAmount = value;
        }
    }
    private float BackgroundAmount
    {
        get
        {
            if (fillType == FillType.Fill) return bar.fillAmount;
            else return bar.color.a;
        }
        set
        {
            if (fillType == FillType.Fill) bar.fillAmount = replaceParentWithFill ? (1 - value) : value;
            else bar.color = new(bar.color.r, bar.color.g, bar.color.b, 
                replaceParentWithFill ? (1 - value) : value);
        }
    }

    private void Awake()
    {
        bar = GetComponent<Image>();
        if (replaceParentWithFill) EnsureParentIsInverse();
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
        float normalizedValue = value / MaxValue;
        BackgroundAmount = normalizedValue;
        filler.fillAmount = normalizedValue;
        OnValueUpdated?.Invoke(normalizedValue);
    }

    IEnumerator UpdateBarCorroutine(float target)
    {
        float speed = (target - FillerAmount) / updateTime;
        if(!replaceParentWithFill) BackgroundAmount = target;
        while (FillerAmount < target)
        {
            FillerAmount = Mathf.MoveTowards(FillerAmount, target, speed * Time.deltaTime);
            yield return null;
        }
        FillerAmount = target;
    }

    private void EnsureParentIsInverse()
    {
        bar.fillMethod = filler.fillMethod; 
        if (filler.fillMethod == Image.FillMethod.Radial360)
            bar.fillClockwise = !filler.fillClockwise;
        if (filler.fillOrigin == (int)Image.OriginHorizontal.Left)
            bar.fillOrigin = (int)Image.OriginHorizontal.Right;
        else if (filler.fillOrigin == (int)Image.OriginHorizontal.Right)
            bar.fillOrigin = (int)Image.OriginHorizontal.Left;
        else if (filler.fillOrigin == (int)Image.OriginVertical.Top)
            bar.fillOrigin = (int)Image.OriginVertical.Bottom;
        else if (filler.fillOrigin == (int)Image.OriginVertical.Bottom)
            bar.fillOrigin = (int)Image.OriginVertical.Top;
    }

    private enum FillType
    {
        Fill,
        Alpha
    }
}
