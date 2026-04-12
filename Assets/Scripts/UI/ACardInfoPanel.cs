using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class ACardInfoPanel : MonoBehaviour
{
    [Min(0.01f)][SerializeField] private float fadeTime = 0.5f;
    [Min(0)][SerializeField] private float showTime;
    [SerializeField] private CardVisualPresenter cardVisualPresenter;
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

    protected void ShowCard(CardDefinition cardDefinition)
    {
        Action onStart = () =>
        {
            bool usePresenter = cardDefinition != null && cardVisualPresenter;
            SetVisualMode(usePresenter);

            if (usePresenter)
            {
                int resolvedManaCost = Mathf.Max(0, cardDefinition.ManaCost);
                cardVisualPresenter.SetCard(cardDefinition, resolvedManaCost, resolvedManaCost);
            }
            else
            {
                cardVisualPresenter?.Clear();
                if (image)
                {
                    image.sprite = cardDefinition ? cardDefinition.Image : null;
                }
            }

            OnImageSet();
        };
        dissolveEffect.FadeInAndOut(fadeTime, showTime, onStart, null);
    }

    private void SetVisualMode(bool usePresenter)
    {
        if (image)
        {
            if (image.gameObject == gameObject)
                image.enabled = !usePresenter;
        }

        if (cardVisualPresenter)
        {
            if (cardVisualPresenter.gameObject == gameObject)
                cardVisualPresenter.enabled = usePresenter;
            else
                cardVisualPresenter.gameObject.SetActive(usePresenter);
        }
    }
}
