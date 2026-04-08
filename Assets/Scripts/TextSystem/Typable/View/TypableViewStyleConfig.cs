using UnityEngine;

namespace TypTyp.TextSystem.Typable
{
    [System.Serializable]
    public struct TypableViewStyleConfig
    {
        public Color CorrectColor;
        public Color WrongColor;
        public bool UnderlineNext;
        public bool RandomizeCorrectColorOnComplete;
        public bool isAbleToShowSpaces;

        public static TypableViewStyleConfig Default => new()
        {
            CorrectColor = Color.white,
            WrongColor = Color.red,
            UnderlineNext = true,
            RandomizeCorrectColorOnComplete = false,
            isAbleToShowSpaces = false
    };
    }
}
