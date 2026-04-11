using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class ACardInfoPanel : MonoBehaviour
{
    [Min(0.01f)][SerializeField] private float fadeTime = 0.5f;
    [Min(0)][SerializeField] private float showTime;
    private CardDissolveEffect dissolveEffect;
    private Image image;

    protected virtual void Awake()
    {
        if(!TryGetComponent(out dissolveEffect)) 
            Debug.LogError("Error: no hay efecto dissolve asociado");
        image = GetComponent<Image>();
        GameUIConfigurator.OnUIConfigurated += PerformSubscriptions;
    }

    protected virtual void OnDestroy() => GameUIConfigurator.OnUIConfigurated -= PerformSubscriptions;

    protected abstract void PerformSubscriptions();

    protected abstract void OnImageSet();

    protected void ShowCard(Sprite cardImage)
    {
        Action onStart = () =>
        {
            image.sprite = cardImage;
            OnImageSet();
        };
        dissolveEffect.FadeInAndOut(fadeTime, showTime, onStart, null);
    }
}
