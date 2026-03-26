using System.Collections.Generic;
using TMPro;
using TypTyp;
using UnityEngine;

[CreateAssetMenu(fileName = "TextFontEffect", menuName = "TypTyp/Effects/TextFontEffect")]
public class TextFontEffect : StatusEffectDefinition
{
    [SerializeField] private TMP_FontAsset font;
    private static readonly Dictionary<string, List<TMP_FontAsset>> activeFonts = new();

    public override void OnActivate(Player target)
    {
        foreach(var t in target.GetComponentsInChildren<TMP_Text>(true))
        {
            t.font = font;
        }
        if (!activeFonts.ContainsKey(Settings.Instance.P1_tag))
        {
            activeFonts.Add(Settings.Instance.P1_tag, new());
            activeFonts.Add(Settings.Instance.P2_tag, new());
        }
        activeFonts[target.tag].Add(font);
    }

    public override void OnDeactivate(Player target)
    {
        activeFonts[target.tag].Remove(font);
        foreach (var t in target.GetComponentsInChildren<TMP_Text>(true))
        {
            t.font = activeFonts[target.tag].Count == 0 ? 
                Settings.Instance.DefaultFont : activeFonts[target.tag][^1];
        }
    }

    public override string GetDefaultValue()
    {
        return "";
    }
}
