using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBar : MonoBehaviour, IFillableBar
{
    [SerializeField] private Image filler;
    [SerializeField] private float updateTime = 0.5f;
    private Image bar;
    public float MaxValue { get; set; } = 1f;


    private void Awake()
    {
        bar = GetComponent<Image>();
    }

    public void UpdateValue(float oldValue, float newValue)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateBarCorroutine(newValue / MaxValue));
    }

    public void SetValueWithoutTransition(float value)
    {
        StopAllCoroutines();
        bar.fillAmount = value;
        filler.fillAmount = value;
    }

    IEnumerator UpdateBarCorroutine(float target)
    {
        float speed = (target - filler.fillAmount) / updateTime;
        bar.fillAmount = target;
        while (filler.fillAmount < target)
        {
            filler.fillAmount = Mathf.MoveTowards(filler.fillAmount, target, speed * Time.deltaTime);
            yield return null;
        }
        filler.fillAmount = target;
    }
}
