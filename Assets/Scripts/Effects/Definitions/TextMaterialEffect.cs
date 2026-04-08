using System;
using System.Collections.Generic;
using TMPro;
using TypTyp;
using UnityEngine;

[CreateAssetMenu(fileName = "TextMaterialEffect", menuName = "TypTyp/Effects/TextMaterialEffect")]
public class TextMaterialEffect : StatusEffectDefinition
{
    [SerializeField] private Material mat;
    [SerializeField] private string textComponentName;
    private Material defaultMat;
    private static readonly Dictionary<string, List<Material>> activeMats = new();

    public override void OnActivate(Player target)
    {
        if (defaultMat == null) defaultMat = target.GetComponentInChildren<TMP_Text>(true).fontMaterial;
        foreach(var t in target.GetComponentsInChildren<TMP_Text>(true))
        {
            AssignMaterial(t, mat);
            if (!string.IsNullOrEmpty(textComponentName))
            {
                Type typeToAdd = Type.GetType(textComponentName);
                if (typeToAdd != null)
                    t.gameObject.AddComponent(typeToAdd);
            }
        }
        if (!activeMats.ContainsKey(Settings.Instance.P1_tag))
        {
            activeMats.Add(Settings.Instance.P1_tag, new());
            activeMats.Add(Settings.Instance.P2_tag, new());
        }
        activeMats[target.tag].Add(mat);
    }

    public override void OnDeactivate(Player target)
    {
        activeMats[target.tag].Remove(mat);
        foreach (var t in target.GetComponentsInChildren<TMP_Text>(true))
        {
            AssignMaterial(t, activeMats[target.tag].Count == 0 ? 
                defaultMat : activeMats[target.tag][^1]);
            if (!string.IsNullOrEmpty(textComponentName))
            {
                Type typeToRemove = Type.GetType(textComponentName);
                Component comp = t.gameObject.GetComponent(typeToRemove);
                Destroy(comp);
            }
        }
    }

    public override string GetDefaultValue()
    {
        return "";
    }

    private void AssignMaterial(TMP_Text t, Material mat)
    {
        Material matInstance = new(mat);
        Texture2D atlas = t.fontMaterial.GetTexture("_MainTex") as Texture2D;
        matInstance.SetTexture("_MainTex", atlas);
        matInstance.SetColor("_FaceColor", t.fontMaterial.GetColor("_FaceColor"));
        matInstance.SetColor("_OutlineColor", t.fontMaterial.GetColor("_OutlineColor"));
        matInstance.SetFloat("_OutlineWidth", t.fontMaterial.GetFloat("_OutlineWidth"));
        t.fontMaterial = matInstance;
    }
}
