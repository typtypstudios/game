using UnityEngine;

public static class Utils
{
    public static string ColorToTag(Color c)
    {
        return $"<color #{ColorUtility.ToHtmlStringRGB(c)}>";
    }
}
