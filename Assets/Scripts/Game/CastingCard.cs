using System.Collections.Generic;
using System.Linq;
using TypTyp.Input;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CastingCard : MonoBehaviour
{
    [SerializeField] private Sprite placeholderSprite;
    [SerializeField] private CardUIManager cardUIManager;
    [SerializeField] private float showTime = 0.5f;
    [SerializeField] private float disappearTime = 0.5f;
    [SerializeField] private float appearTime = 1;
    private Animator anim;
    private bool showingCard = false;
    private readonly Dictionary<CardUI, float> progressDictionary = new();
    private Image image;
    private CardDissolveEffect dissolveEffect;
    private readonly Queue<Sprite> completedQueue = new();

    private void Awake()
    {
        if (!TryGetComponent(out dissolveEffect)) 
            Debug.LogError("Error: falta el componente CardDissolveEffect");
        image = GetComponent<Image>();
        image.sprite = placeholderSprite;
        anim = GetComponent<Animator>();
        InputHandler.Instance.OnInputModeChanged += HandleModeChange;
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
        InputHandler.Instance.OnInputModeChanged -= HandleModeChange;
    }

    private void HandleModeChange(InputModeMask mask)
    {
        if (mask != InputModeMask.Spells && !showingCard)
            dissolveEffect.SetDissolve(1, true, disappearTime);
    }

    private void OnCardUpdated(CardUI card, float progress, bool canBeCasted)
    {
        if (canBeCasted && showingCard && Mathf.Approximately(progress, 1))
            completedQueue.Enqueue(card.CardDefinition.Image);
        if (!canBeCasted || showingCard) return;
        progressDictionary[card] = progress;
        float max = progressDictionary.Values.Max();
        dissolveEffect.SetDissolve(1 - max, true, appearTime);
        if (Mathf.Approximately(progressDictionary[card], 1))
            ShowCard(card.CardDefinition.Image);
    }

    private void ShowCard(Sprite sprite)
    {
        image.sprite = sprite;
        anim.SetTrigger("ShowCard");
        showingCard = true;
    }

    public void OnAnimEnded()
    {
        dissolveEffect.FadeInAndOut(disappearTime, showTime, null, OnCardDisappear, false);
        foreach (var key in progressDictionary.Keys.ToList())
            progressDictionary[key] = 0;
    }

    private void OnCardDisappear()
    {
        showingCard = false;
        image.sprite = placeholderSprite;
        if (completedQueue.Count > 0)
        {
            dissolveEffect.SetDissolve(0);
            ShowCard(completedQueue.Dequeue());
        }
    }
}
