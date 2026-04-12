using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UIBar))]
public class TutorialBarFiller : MonoBehaviour
{
    [SerializeField] private float changeTime = 0.5f;
    [SerializeField] private float[] values = new float[5] { 0f, 0.25f, 0.5f, 0.75f, 1 };
    private UIBar uiBar;
    private int idx = 0;
    private WaitForSeconds wait;

    void Awake()
    {
        uiBar = GetComponent<UIBar>();
        wait = new(changeTime);
    }

    private void OnEnable()
    {
        StartCoroutine(ChangeFillCoroutine());
    }

    IEnumerator ChangeFillCoroutine()
    {
        while (true)
        {
            yield return wait;
            if (++idx >= values.Length) idx = 0;
            uiBar.SetValueWithoutTransition(values[idx]);
        }
    }
}
