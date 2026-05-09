using System.Collections;
using UnityEngine;

public class TutorialBarFiller : MonoBehaviour
{
    [SerializeField] private FillType fillType = FillType.Snap;
    [Header("Snap fill:")]
    [SerializeField] private float changeTime = 0.5f;
    [SerializeField] private float[] values = new float[5] { 0f, 0.25f, 0.5f, 0.75f, 1 };
    [Header("Continuous fill:")]
    [SerializeField] private float fillTime = 2.0f;
    [SerializeField] private float cooldownOnFill = 2.0f;
    private UIBar uiBar;
    private UIBarGroup uiBarGroup;
    private int idx = 0;
    private WaitForSeconds wait;

    void Awake()
    {
        uiBar = GetComponent<UIBar>();
        uiBarGroup = GetComponent<UIBarGroup>();
        wait = new(changeTime);
    }

    private void OnEnable()
    {
        if (fillType == FillType.Snap) StartCoroutine(SnapFillCoroutine());
        else StartCoroutine(ContinuousFillCoroutine());
    }

    IEnumerator SnapFillCoroutine()
    {
        while (true)
        {
            yield return wait;
            if (++idx >= values.Length) idx = 0;
            uiBar?.SetValueWithoutTransition(values[idx]);
            uiBarGroup?.UpdateValue(0, values[idx]);
        }
    }

    IEnumerator ContinuousFillCoroutine()
    {
        float speed = 1f / fillTime;
        float currentValue = 0;
        while (true)
        {
            uiBar?.SetValueWithoutTransition(currentValue);
            uiBarGroup?.UpdateValue(0, currentValue);
            if (currentValue >= 1)
            {
                yield return new WaitForSeconds(cooldownOnFill);
                currentValue = 0;
            }
            currentValue += speed * Time.deltaTime;
            yield return null;
        }
    }

    private enum FillType
    {
        Snap,
        Continuous
    }
}
