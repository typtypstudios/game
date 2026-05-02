using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Mask))]
public class ScratchAnimation : MonoBehaviour
{
    [Min(0.01f)][field: SerializeField] public float AnimTime { get; private set; } = 0.3f;
    [SerializeField] private float delayTime = 0f;
    [SerializeField] private ScratchOrigin appearOrigin;
    [SerializeField] private ScratchOrigin disappearOrigin;
    private Image image;
    public event Action OnScratch;
    public event Action OnScratchRemoved;
    private Coroutine scratchCoroutine;

    void Awake()
    {
        image = GetComponent<Image>();
        image.fillAmount = 0;
    }

    public void SetScratchAmount(int value) => image.fillAmount = value;

    public void Scratch(bool resetValue = true)
    {
        if(resetValue) image.fillAmount = 0;
        image.fillOrigin = GetFillOrigin(appearOrigin);
        if(scratchCoroutine != null) StopCoroutine(scratchCoroutine);
        scratchCoroutine = StartCoroutine(ScratchCoroutine());
    }

    public void RemoveScratch(bool resetValue = true)
    {
        if (resetValue) image.fillAmount = 1;
        image.fillOrigin = GetFillOrigin(disappearOrigin);
        if (scratchCoroutine != null) StopCoroutine(scratchCoroutine);
        scratchCoroutine = StartCoroutine(RemoveScratchCoroutine());
    }

    public void ScratchAndRemove(float timeInterval, float waitTime = 0)
    {
        StopAllCoroutines();
        StartCoroutine(ScratchAndRemoveCoroutine(timeInterval, waitTime));
    }

    private int GetFillOrigin(ScratchOrigin origin)
    {
        return origin == ScratchOrigin.Default ? (int)Image.OriginHorizontal.Right :
            (int)Image.OriginHorizontal.Left;
    }

    IEnumerator ScratchCoroutine()
    {
        yield return new WaitForSeconds(delayTime);
        float speed = (1 - image.fillAmount) / AnimTime;
        while(image.fillAmount < 1)
        {
            image.fillAmount += speed * Time.deltaTime;
            yield return null;
        }
        OnScratch?.Invoke();
    }

    IEnumerator RemoveScratchCoroutine()
    {
        float speed = image.fillAmount / AnimTime;
        while (image.fillAmount > 0)
        {
            image.fillAmount -= speed * Time.deltaTime;
            yield return null;
        }
        OnScratchRemoved?.Invoke();
    }

    IEnumerator ScratchAndRemoveCoroutine(float intervalTime, float waitTime)
    {
        yield return new WaitForSeconds(waitTime + delayTime);
        Scratch();
        yield return new WaitForSeconds(intervalTime + AnimTime);
        RemoveScratch();
    }
}

public enum ScratchOrigin
{
    Default,
    Inverse
}