using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CastingCard : MonoBehaviour
{
    [SerializeField] private Sprite placeholderSprite;
    [SerializeField] private CardUIManager cardUIManager;
    [SerializeField] private float showTime = 0.5f;
    [SerializeField] private float disappearTime = 0.5f;
    [SerializeField] private float appearSpeed = 1;
    private Animator anim;
    private bool showingCard = false;
    private readonly Dictionary<CardUI, float> progressDictionary = new();
    private Image image;
    private CardDissolveEffect dissolveEffect;

    private void Awake()
    {
        if (!TryGetComponent(out dissolveEffect)) 
            Debug.LogError("Error: falta el componente CardDissolveEffect");
        image = GetComponent<Image>();
        image.sprite = placeholderSprite;
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        foreach(var card in cardUIManager.GetComponentsInChildren<CardUI>())
        {
            card.OnIdxChanged += OnCardUpdated;
        }
    }

    private void OnDestroy()
    {
        foreach (var card in cardUIManager.GetComponentsInChildren<CardUI>())
        {
            card.OnIdxChanged -= OnCardUpdated;
        }
    }

    private void OnCardUpdated(CardUI card, float progress, bool canBeCasted)
    {
        //if (progress == 1 && showingCard)
        //{

        //}
        if (!canBeCasted || showingCard) return;
        image.sprite = placeholderSprite;
        progressDictionary[card] = progress;
        float max = progressDictionary.Values.Max();
        dissolveEffect.SetDissolve(1 - max, true, appearSpeed);
        if (Mathf.Approximately(progressDictionary[card], 1))
        {
            image.sprite = card.CardDefinition.Image;
            anim.SetTrigger("ShowCard");
            showingCard = true;
        }
    }

    public void OnAnimEnded()
    {
        dissolveEffect.FadeInAndOut(disappearTime, showTime, null, OnCardDisappear, false);
    }

    private void OnCardDisappear()
    {
        showingCard = false;
    }
}
