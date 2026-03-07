using TMPro;
using TypTyp;
using UnityEngine;

[CreateAssetMenu(fileName = "TextFontEffect", menuName = "TypTyp/Effects/TextFontEffect")]
public class TextFontEffect : StatusEffectDefinition
{
    [SerializeField] private TMP_FontAsset font;

    public override void OnActivate(Player target)
    {
        foreach(var t in target.GetComponentsInChildren<TMP_Text>(true))
        {
            t.font = font;
        }
        target.StatusEffectController.ActiveFonts.Add(font);
    }

    public override void OnDeactivate(Player target)
    {
        target.StatusEffectController.ActiveFonts.Remove(font);
        foreach (var t in target.GetComponentsInChildren<TMP_Text>(true))
        {
            t.font = target.StatusEffectController.ActiveFonts.Count == 0 ? 
                Settings.Instance.DefaultFont : target.StatusEffectController.ActiveFonts[^1];
        }
    }
}
