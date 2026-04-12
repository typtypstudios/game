using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererCardView : MonoBehaviour, ICardView
{
    [Header("Main Layers")]
    [SerializeField] private SpriteRenderer illustrationRenderer;
    [SerializeField] private SpriteRenderer frameRenderer;
    [SerializeField] private SpriteRenderer levelDetailRenderer;

    [Header("Orbs")]
    [SerializeField] private Transform orbContainer;
    [SerializeField] private SpriteRenderer orbPrefab;
    [SerializeField] private List<SpriteRenderer> orbs = new();

    public void Apply(CardVisualState state)
    {
        if (illustrationRenderer)
        {
            illustrationRenderer.sprite = state.IllustrationSprite;
            illustrationRenderer.color = state.CardTint;
        }

        if (frameRenderer)
        {
            frameRenderer.sprite = state.FrameSprite;
            frameRenderer.color = state.FrameColor;
        }

        if (levelDetailRenderer)
        {
            levelDetailRenderer.sprite = state.LevelDetailSprite;
            levelDetailRenderer.gameObject.SetActive(state.LevelDetailSprite);
            if (state.LevelDetailSprite)
            {
                levelDetailRenderer.color = Color.white;
            }
        }

        RenderOrbs(state);
    }

    public void Clear()
    {
        if (illustrationRenderer)
        {
            illustrationRenderer.sprite = null;
            illustrationRenderer.color = Color.white;
        }

        if (frameRenderer)
        {
            frameRenderer.color = Color.white;
        }

        if (levelDetailRenderer)
        {
            levelDetailRenderer.sprite = null;
            levelDetailRenderer.gameObject.SetActive(false);
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
            orb.color = Color.white;
        }
    }

    private void EnsureOrbPool(int count)
    {
        while (orbs.Count < count)
        {
            var newOrb = Instantiate(orbPrefab, orbContainer);
            orbs.Add(newOrb);
        }
    }
}
