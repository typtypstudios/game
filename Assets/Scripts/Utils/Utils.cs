using System;
using System.Security.Cryptography;
using System.Text;
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
}
