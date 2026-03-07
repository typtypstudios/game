using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBar : MonoBehaviour
{
    [SerializeField] private Image filler;
    [SerializeField] private float updateSpeed = 3f;
    private Image bar;
    public UIBar PrevBar { get; set; }
    public float FillAmount => filler.fillAmount;
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
        bar.fillAmount = target;
        if(PrevBar != null) 
            while (PrevBar.FillAmount != 1) yield return null;
        while(filler.fillAmount != target)
        {
            filler.fillAmount = Mathf.MoveTowards(filler.fillAmount, target, updateSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
