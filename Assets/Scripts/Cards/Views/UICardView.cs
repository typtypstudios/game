using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICardView : MonoBehaviour, ICardView
{
    [Header("Main Layers")]
    [SerializeField] private Image illustrationImage;
    [SerializeField] private Image frameImage;
    [SerializeField] private Image levelDetailImage;

    [Header("Orbs")]
    [SerializeField] private Transform orbContainer;
    [SerializeField] private Image orbPrefab;
    [SerializeField] private List<Image> orbs = new();

    void Awake()
    {
        EnsureOrbPool(4);
    }

    public void Apply(CardVisualState state)
    {
        if (illustrationImage)
        {
            illustrationImage.sprite = state.IllustrationSprite;
            illustrationImage.color = state.CardTint;
        }

        if (frameImage)
        {
            frameImage.sprite = state.FrameSprite;
            frameImage.color = state.FrameColor * state.CardTint;
        }

        if (levelDetailImage)
        {
            levelDetailImage.sprite = state.LevelDetailSprite;
            levelDetailImage.gameObject.SetActive(state.LevelDetailSprite);
            if (state.LevelDetailSprite)
            {
                levelDetailImage.color = state.CardTint;
            }
        }

        RenderOrbs(state);
    }

    public void Clear()
    {
        if (illustrationImage)
        {
            illustrationImage.sprite = null;
            illustrationImage.color = Color.white;
        }

        if (frameImage)
        {
            frameImage.color = Color.white;
        }

        if (levelDetailImage)
        {
            levelDetailImage.sprite = null;
            levelDetailImage.gameObject.SetActive(false);
        }

        for (int i = 0; i < orbs.Count; i++)
        {
            if (orbs[i])
            {
                orbs[i].gameObject.SetActive(false);
            }
        }
    }

    private void RenderOrbs(CardVisualState state)
    {
        if (!orbContainer || !orbPrefab)
        {
            return;
        }

        EnsureOrbPool(state.ActiveOrbCount);

        for (int i = 0; i < orbs.Count; i++)
        {
            var orb = orbs[i];
            if (!orb)
            {
                continue;
            }

            bool shouldBeActive = i < state.ActiveOrbCount;
            orb.gameObject.SetActive(shouldBeActive);
            if (!shouldBeActive)
            {
                continue;
            }

            bool isFilled = i < state.FilledOrbCount;
            orb.sprite = isFilled ? state.FilledOrbSprite : state.EmptyOrbSprite;
            orb.color = state.CardTint;
        }
    }

    private void EnsureOrbPool(int count)
    {
        while (orbs.Count < count)
        {
            var newOrb = Instantiate(orbPrefab, orbContainer);
            newOrb.gameObject.SetActive(false);
            orbs.Add(newOrb);
        }
    }
}
