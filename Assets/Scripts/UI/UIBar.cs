using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBar : MonoBehaviour
{
    [SerializeField] private float updateSpeed = 3f;
    public float MaxValue { get; set; } = 1f;
    private Image bar;

    private void Awake()
    {
        bar = GetComponent<Image>();
    }

    public void UpdateValue(float oldValue, float newValue)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateBarCorroutine(newValue / MaxValue));
    }

    IEnumerator UpdateBarCorroutine(float target)
    {
        while(bar.fillAmount != target)
        {
            bar.fillAmount = Mathf.MoveTowards(bar.fillAmount, target, updateSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
