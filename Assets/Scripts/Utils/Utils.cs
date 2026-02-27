using UnityEngine;

public static class Utils
{
    public static string ColorToTag(Color c)
    {
        return $"<color #{ColorUtility.ToHtmlStringRGB(c)}>";
    }

    public static Color GetDifferentColor(Color c, float threshold = 0.2f)
    {
        Vector3 newColorVec;
        Vector3 cVec = new(c.r, c.g, c.b);
        do
        {
            newColorVec = new(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        } while (Mathf.Abs((newColorVec - cVec).magnitude) < threshold);
        return new(newColorVec.x, newColorVec.y, newColorVec.z);
    }
}
