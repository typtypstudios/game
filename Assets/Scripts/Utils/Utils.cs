using TMPro;
using TypTyp;
using UnityEngine;

public static class Utils
{
    public static string ColorToTag(Color c)
    {
        return $"<color #{ColorUtility.ToHtmlStringRGB(c)}>";
    }

    public static Color GetDifferentColor(Color c, int seed = -1, float threshold = 0.2f)
    {
        System.Random rng = seed != -1 ? new(seed) : new((int)System.DateTime.Now.Ticks);
        Vector3 newColorVec;
        Vector3 cVec = new(c.r, c.g, c.b);
        do
        {
            newColorVec = new((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble());
        } while (Mathf.Abs((newColorVec - cVec).magnitude) < threshold);
        return new(newColorVec.x, newColorVec.y, newColorVec.z);
    }

    public static int GetSeedFromNames()
    {
        string p1Name = GameObject.FindGameObjectWithTag(Settings.Instance.P1_tag).name;
        string p2Name = GameObject.FindGameObjectWithTag(Settings.Instance.P2_tag).name;
        return p1Name.GetHashCode() + p2Name.GetHashCode(); 
    }

    public static Transform FindChildrenWithTag(Transform t, string tag)
    {
        foreach(Transform child in t.GetComponentsInChildren<Transform>())
        {
            if (child == t) continue;
            if (child.CompareTag(tag)) return child;
            FindChildrenWithTag(child, tag);
        }
        return null;
    }

    public static float RandomInRange(Vector2 range)
    {
        return Random.Range(range.x, range.y);
    }

    public static string ApplyColorToText(string text, Color color)
    {
        return $"{ColorToTag(color)}{text}</color>";
    }

    public static Color ColorToHDR(Color color, float intensity)
    {
        return color * Mathf.Pow(2, intensity);
    }

    public static string ParseUnicodeEscapes(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, @"\\u([0-9a-fA-F]{4})",
            m => ((char)System.Convert.ToInt32(m.Groups[1].Value, 16)).ToString());
    }

    public static void SetMaterialTMP(TMP_Text t, Material mat)
    {
        Material matInstance = new(mat);
        Texture2D atlas = t.fontMaterial.GetTexture("_MainTex") as Texture2D;
        matInstance.SetTexture("_MainTex", atlas);
        matInstance.SetColor("_FaceColor", t.fontMaterial.GetColor("_FaceColor"));
        //matInstance.SetColor("_OutlineColor", t.fontMaterial.GetColor("_OutlineColor"));
        //matInstance.SetFloat("_OutlineWidth", t.fontMaterial.GetFloat("_OutlineWidth"));
        t.fontMaterial = matInstance;
    }

    public static void ChangeLayerToHierarchy(Transform parent, LayerMask layer)
    {
        parent.gameObject.layer = layer;
        for (int i = 0; i < parent.childCount; i++)
        {
            ChangeLayerToHierarchy(parent.GetChild(i), layer);
        }
    }
}
