using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Mask))]
public class ScratchAnimation : MonoBehaviour
{
    [Min(0.01f)][SerializeField] private float animTime;
    [SerializeField] private ScratchOrigin appearOrigin;
    [SerializeField] private ScratchOrigin disappearOrigin;
    private Image image;
    public event Action OnScratch;
    public event Action OnScratchRemoved;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Scratch(bool resetValue = true)
    {
        if(resetValue) image.fillAmount = 0;
        image.fillOrigin = GetFillOrigin(appearOrigin);
        StopAllCoroutines();
        StartCoroutine(ScratchCoroutine());
    }

    public void RemoveScratch(bool resetValue = true)
    {
        if (resetValue) image.fillAmount = 1;
        image.fillOrigin = GetFillOrigin(disappearOrigin);
        StopAllCoroutines();
        StartCoroutine(RemoveScratchCoroutine());
    }

    private int GetFillOrigin(ScratchOrigin origin)
    {
        return origin == ScratchOrigin.Default ? (int)Image.OriginHorizontal.Right :
            (int)Image.OriginHorizontal.Left;
    }

    IEnumerator ScratchCoroutine()
    {
        float speed = (1 - image.fillAmount) / animTime;
        while(image.fillAmount < 1)
        {
            image.fillAmount += speed * Time.deltaTime;
            yield return null;
        }
        OnScratch?.Invoke();
    }

    IEnumerator RemoveScratchCoroutine()
    {
        float speed = image.fillAmount / animTime;
        while (image.fillAmount > 0)
        {
            image.fillAmount -= speed * Time.deltaTime;
            yield return null;
        }
        OnScratchRemoved?.Invoke();
    }
}

public enum ScratchOrigin
{
    Default,
    Inverse
}