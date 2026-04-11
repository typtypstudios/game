using System.Text;
using UnityEngine;

public class ChatMarkerFormatter
{
    private char spellMarker = '\u0001';

    public char SpellMarker => spellMarker;

    public string Strip(string s) => string.IsNullOrEmpty(s) ? s : s.Replace(spellMarker.ToString(), "");

    public string Wrap(string s) => string.IsNullOrEmpty(s) ? s : spellMarker + s + spellMarker;

    public string ToRich(string s)
    {
        if (string.IsNullOrEmpty(s) || s.IndexOf(spellMarker) < 0) return s;

        string openTag = Utils.ColorToTag(UIColors.Instance.SpellHighlightColor);
        const string closeTag = "</color>";

        int count = 0;
        foreach (char ch in s) if (ch == spellMarker) count++;

        var sb = new StringBuilder(s.Length + 32);
        bool open = false;

        if (count % 2 == 1)
        {
            sb.Append(openTag);
            open = true;
        }

        foreach (char ch in s)
        {
            if (ch == spellMarker)
            {
                if (open) { sb.Append(closeTag); open = false; }
                else { sb.Append(openTag); open = true; }
            }
            else sb.Append(ch);
        }
        if (open) sb.Append(closeTag);
        return sb.ToString();
    }
}