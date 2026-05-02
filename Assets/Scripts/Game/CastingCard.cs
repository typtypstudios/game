using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CastingCard : NetworkBehaviour
{
    [SerializeField] private PlayerInputManager inputManager;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private Sprite placeholderSprite;
    [SerializeField] private CardVisualPresenter cardVisualPresenter;
    [SerializeField] private CardUIManager cardUIManager;
    [SerializeField] private Material enemyMat;
    [Header("Animation")]
    [SerializeField] private float showTime = 0.5f;
    [SerializeField] private float disappearTime = 0.5f;
    [SerializeField] private float appearTime = 1;
    private Animator anim;
    private bool showingCard = false;
    private readonly Dictionary<CardUI, float> progressDictionary = new();
    private Image image;
    private CardDissolveEffect dissolveEffect;
    private readonly Queue<CardDefinition> completedQueue = new();
    private readonly NetworkVariable<float> progress = new(0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        if (!TryGetComponent(out dissolveEffect)) 
            Debug.LogError("Error: falta el componente CardDissolveEffect");
        image = GetComponent<Image>();
        ShowSprite(placeholderSprite);
        anim = GetComponent<Animator>();
        inputManager.OnAnimChanged += HandleAnimChange;
        progress.OnValueChanged += (oldVal, newVal) =>
        {
            if (!IsOwner) dissolveEffect.SetDissolve(1 - newVal, true, appearTime);
        };
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
        inputManager.OnAnimChanged -= HandleAnimChange;
    }

    private void HandleAnimChange(AnimState state)
    {
        if (state != AnimState.Spell && !showingCard)
            dissolveEffect.SetDissolve(1, true, disappearTime);
        else if (state == AnimState.Spell && !IsOwner)
        {
            ShowSprite(backSprite);
            //dissolveEffect.SetDissolve(0, true, appearTime);
            //dissolveEffect.OverrideMaterial(enemyMat);
        }
    }

    private void OnCardUpdated(CardUI card, float progress, bool canBeCasted)
    {
        if (!IsOwner) return;
        if (canBeCasted && showingCard && Mathf.Approximately(progress, 1))
            completedQueue.Enqueue(card.CardDefinition);
        if (!canBeCasted || showingCard) return;
        progressDictionary[card] = progress;
        float max = progressDictionary.Values.Max();
        this.progress.Value = max;
        dissolveEffect.SetDissolve(1 - max, true, appearTime);
        if (Mathf.Approximately(progressDictionary[card], 1))
            ShowCard(card.CardDefinition);
    }

    private void ShowCard(CardDefinition cardDefinition)
    {
        if (!IsOwner) return;
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

        anim.SetTrigger("ShowCard");
        showingCard = true;
    }

    private void ShowSprite(Sprite sprite)
    {
        SetVisualMode(false);
        cardVisualPresenter?.Clear();
        if (image)
        {
            image.sprite = sprite;
        }
    }

    private void SetVisualMode(bool usePresenter)
    {
        if (image)
        {
            if (image.gameObject == gameObject)
                image.enabled = !usePresenter;
            else
                image.gameObject.SetActive(!usePresenter);
        }

        if (cardVisualPresenter)
        {
            if (cardVisualPresenter.gameObject == gameObject)
                cardVisualPresenter.enabled = usePresenter;
            else
                cardVisualPresenter.gameObject.SetActive(usePresenter);
        }
    }

    public void OnAnimEnded()
    {
        if (!IsOwner) return;
        dissolveEffect.FadeInAndOut(disappearTime, showTime, null, OnCardDisappear, false);
        foreach (var key in progressDictionary.Keys.ToList())
            progressDictionary[key] = 0;
    }

    private void OnCardDisappear()
    {
        if (!IsOwner) return;
        showingCard = false;
        ShowSprite(placeholderSprite);
        progress.Value = 0;
        if (completedQueue.Count > 0)
        {
            dissolveEffect.SetDissolve(0);
            progress.Value = 1;
            ShowCard(completedQueue.Dequeue());
        }
    }
}
