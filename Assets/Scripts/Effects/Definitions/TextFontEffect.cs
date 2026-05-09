using System.Collections.Generic;
using TMPro;
using TypTyp;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "TextFontEffect", menuName = "TypTyp/Effects/TextFontEffect")]
public class TextFontEffect : StatusEffectDefinition
{
    [SerializeField] private TMP_FontAsset font;
    private static readonly Dictionary<string, List<TMP_FontAsset>> activeFonts = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        activeFonts.Clear();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        activeFonts.Clear();
    }

    public override void OnActivate(EffectContext context)
    {
        Player target = context.Target;
        foreach(var t in target.GetComponentsInChildren<TMP_Text>(true))
        {
            SafelyChangeFont(t, font);
        }
        if (!activeFonts.ContainsKey(Settings.Instance.P1_tag))
        {
            activeFonts.Add(Settings.Instance.P1_tag, new());
            activeFonts.Add(Settings.Instance.P2_tag, new());
        }
        activeFonts[target.tag].Add(font);
    }

    public override void OnDeactivate(EffectContext context)
    {
        Player target = context.Target;
        activeFonts[target.tag].Remove(font);
        foreach (var t in target.GetComponentsInChildren<TMP_Text>(true))
        {
            SafelyChangeFont(t, activeFonts[target.tag].Count == 0 ?
                Settings.Instance.DefaultFont : activeFonts[target.tag][^1]);
        }
    }

    private void SafelyChangeFont(TMP_Text tmp, TMP_FontAsset font)
    {
        Material initMat = tmp.fontMaterial;
        tmp.font = font;
        Utils.SetMaterialTMP(tmp, initMat);
    }

    public override string GetDefaultValue()
    {
        return "";
    }
}

