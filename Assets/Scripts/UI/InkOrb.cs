using System.Collections;
using UnityEngine;

public class InkOrb : MonoBehaviour, IFillableBar
{
    [field: SerializeField] public float MaxValue { get; set; } = -1f; //Altura máxima
    [SerializeField] private RectTransform bar;
    [SerializeField] private RectTransform filler;
    [SerializeField] private float updateTime = 0.5f;
    private float initHeight;
    public InkOrb PrevOrb { get; set; }
    private float FillHeight {
        get { return filler.anchoredPosition.y; }
        set { SetHeight(filler, value); }
    }

    private void Awake() => initHeight = bar.anchoredPosition.y;

    public void UpdateValue(float oldValue, float newValue)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateBarCorroutine(UnNormalizeHeight(newValue)));
    }

    public void SetValueWithoutTransition(float value)
    {
        StopAllCoroutines();
        SetHeight(bar, UnNormalizeHeight(value));
        SetHeight(filler, UnNormalizeHeight(value));
    }

    private float UnNormalizeHeight(float t)
    {
        return Mathf.Lerp(initHeight, MaxValue, t);
    }

    private void SetHeight(RectTransform rt, float height) => 
        rt.anchoredPosition = new(rt.anchoredPosition.x, height);

    IEnumerator UpdateBarCorroutine(float target)
    {
        SetHeight(bar, target);
        float speed = (target - FillHeight) / updateTime;
        while (FillHeight < target)
        {
            FillHeight = Mathf.MoveTowards(FillHeight, target, speed * Time.deltaTime);
            yield return null;
        }
        FillHeight = target;
    }
}
