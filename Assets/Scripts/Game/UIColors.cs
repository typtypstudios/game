using UnityEngine;

[NoAutoCreate]
[CreateAssetMenu(fileName = nameof(UIColors), menuName = "TypTyp/UIColors")]
public class UIColors : ScriptableSingleton<UIColors>
{
    [field: SerializeField] public Color DurationHighlightColor { get; set; } = Color.cyan;
    [field: SerializeField] public Color PositiveHighlightColor { get; set; } = Color.green;
    [field: SerializeField] public Color NegativeHighlightColor { get; set; } = Color.red;
    [field: SerializeField] public Color EffectHighlightColor { get; set; } = Color.purple;
    [field: SerializeField] public Color DevotionPointsColor { get; set; } = Color.yellowNice;
}
