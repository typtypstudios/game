using UnityEngine;

public class SimpleTextProvider : MonoBehaviour, ITextProvider
{
    int index = 0;
    public string[] texts = new string[]
    {
        "The ritual is complete.",
        "You feel a surge of power.",
        "The air around you crackles with energy.",
        "A dark presence looms over you.",
        "You have unlocked a new ability."
    };

    public string GetNextText() => texts[index++];
}
