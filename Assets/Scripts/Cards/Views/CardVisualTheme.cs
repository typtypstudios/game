using UnityEngine;

[CreateAssetMenu(fileName = "CardVisualTheme", menuName = "TypTyp/Cards/CardVisualTheme")]
public class CardVisualTheme : ScriptableObject
{
    [Header("Main Layers")]
    [SerializeField] private Sprite frameSprite;
    [SerializeField] private Sprite fallbackIllustration;
    [SerializeField] private Sprite[] levelDetailByRequiredLevel;

    [Header("Orb Layers")]
    [SerializeField] private Sprite filledOrbSprite;
    [SerializeField] private Sprite emptyOrbSprite;
    [SerializeField, Min(0)] private int maxOrbs = 8;

    [Header("Colors")]
    [SerializeField] private Color fallbackCultColor = Color.white;
    [SerializeField] private Color blockedCardTint = Color.gray;
    [SerializeField] private Color affordableCardTint = Color.white;

    public Sprite FrameSprite => frameSprite;
    public Sprite FallbackIllustration => fallbackIllustration;
    public Sprite FilledOrbSprite => filledOrbSprite;
    public Sprite EmptyOrbSprite => emptyOrbSprite;
    public int MaxOrbs => maxOrbs;
    public Color FallbackCultColor => fallbackCultColor;
    public Color BlockedCardTint => blockedCardTint;
    public Color AffordableCardTint => affordableCardTint;

    public Sprite GetLevelDetailSprite(int requiredLevel)
    {
        if (levelDetailByRequiredLevel == null || levelDetailByRequiredLevel.Length == 0)
        {
            return null;
        }

        int index = Mathf.Clamp(requiredLevel, 0, levelDetailByRequiredLevel.Length - 1);
        return levelDetailByRequiredLevel[index];
    }
}
