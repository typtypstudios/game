using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace TypTyp.TextSystem
{
    public class SourceTextProvider : MonoBehaviour, ITextProvider
    {
        [SerializeField, FormerlySerializedAs("TextSource")] TextAsset textSource;

        private List<string> phrases = new();
        private System.Random random;
        private int currentIndex;

        public int Count => phrases.Count;

        public string GetNextText()
        {
            if (phrases.Count == 0) return string.Empty;
            string text = GetText(currentIndex);
            currentIndex++;
            return text;
        }

        public string GetText(int index)
        {
            if (index < 0 || index >= phrases.Count)
                return string.Empty;

            return phrases[index];
        }

        public void SetRandom(System.Random random)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
            if (phrases != null && phrases.Count > 0)
            {
                RandomizePhrases(this.random);
                currentIndex = 0;
            }
        }

        void Awake()
        {
            LoadSource(textSource);
        }

        private void LoadSource(TextAsset textSource)
        {
            phrases = textSource != null
                ? textSource.text
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(phrase => phrase.Trim())
                    .Where(phrase => !string.IsNullOrWhiteSpace(phrase))
                    .ToList()
                : new();

            if (phrases.Count <= 0)
            {
                Debug.LogError("No text found in the source asset");
                return;
            }
            if (phrases.Count < Settings.Instance.MaxTextsProvided)
            {
                Debug.LogWarning("Not enough text in the source asset. Using default phrases.");
            }
        }

        private void RandomizePhrases(System.Random random)
        {
            phrases.Shuffle(random);
            // UnityEngine.Random.State
        }
    }
}
