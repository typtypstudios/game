using UnityEngine;

namespace TypTyp.TextSystem
{
    public class SimpleTextProvider : MonoBehaviour, ITextProvider
    {
        [SerializeField]
        string[] sampleSentences =
        {
        "The quick brown fox jumps over the lazy dog.",
        "Lorem ipsum dolor sit amet",
        "Unity is a powerful game development platform.",
        "C# is a versatile programming language."
        };

        private int currentIndex = 0;

        public string GetNextText()
        {
            if (currentIndex >= sampleSentences.Length)
                currentIndex = 0;

            return sampleSentences[currentIndex++];
        }
    }
}
