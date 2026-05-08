using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TypTyp.TextSystem
{
    public class SourceTextProvider : MonoBehaviour, ITextProvider
    {
        [SerializeField] TextAsset TextSource;

        private List<string> phrases;
        private System.Random random = new();

        public string GetNextText() => default;

        public string GetText(int index) => default;

        public void SetRandom(System.Random random)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
            if (phrases != null && phrases.Count > 0)
            {
                RandomizePhrases(this.random);
            }
        }

        void Awake()
        {
            LoadSource(TextSource);
            if (phrases != null && phrases.Count > 0)
            {
                RandomizePhrases(random);
            }
        }

        private void LoadSource(TextAsset textSource)
        {
            phrases = textSource != null
                ? textSource.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList()
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
