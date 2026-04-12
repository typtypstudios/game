using UnityEngine;

public static class CardVisualStateBuilder
{
    public static CardVisualState Build(CardDefinition card, CardVisualTheme theme, int manaCost, int currentMana)
    {
        int clampedManaCost = Mathf.Max(0, manaCost);
        int activeOrbCount = Mathf.Clamp(clampedManaCost, 0, theme.MaxOrbs);
        int filledOrbCount = Mathf.Clamp(currentMana, 0, activeOrbCount);
        bool canAfford = currentMana >= clampedManaCost;

        var state = new CardVisualState
        {
            IllustrationSprite = card && card.Image ? card.Image : theme.FallbackIllustration,
            FrameSprite = theme.FrameSprite,
            LevelDetailSprite = card ? theme.GetLevelDetailSprite(card.RequiredLevel) : null,
            FilledOrbSprite = theme.FilledOrbSprite,
            EmptyOrbSprite = theme.EmptyOrbSprite,
            FrameColor = card && card.Cult ? card.Cult.Color : theme.FallbackCultColor,
            CardTint = canAfford ? theme.AffordableCardTint : theme.BlockedCardTint,
            ActiveOrbCount = activeOrbCount,
            FilledOrbCount = filledOrbCount
        };

        return state;
    }
}
