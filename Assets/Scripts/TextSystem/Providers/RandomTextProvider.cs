using System;
using System.Collections.Generic;
using UnityEngine;

namespace TypTyp.TextSystem
{
    public enum GenerationMode { None, Cycle, Random }

    public class RandomTextProvider : MonoBehaviour, ITextProvider
    {
        [SerializeField] TextAsset textSource;
        [SerializeField] GenerationMode generationMode = GenerationMode.Cycle;
        [SerializeField, Min(1)] int repetitionsPerCycle = 1;

        [SerializeField]string[] phrases;
        private System.Random random = new();

        //Just for cycle mode
        int currentIndex = 0;
        int[] indexer;

        void Awake()
        {
            LoadSource();
            Initialize();
        }

        void LoadSource()
        {
            phrases = textSource != null
                ? textSource.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                : Array.Empty<string>();
        }

        void Initialize()
        {
            if (phrases.Length == 0)
            {
                return;
            }

            if(generationMode == GenerationMode.Cycle)
            {
                indexer = RangeProvider.FillRepeatedRange(0, phrases.Length - 1, repetitionsPerCycle);
                indexer.Shuffle(random);
            }
        }

        public string GetNextText()
        {
            if(generationMode == GenerationMode.Cycle)
            {
                if (currentIndex >= indexer.Length)
                {
                    //El reshuffle se podria hacer en async al terminar el ciclo
                    indexer.Shuffle(random);
                    currentIndex = 0;
                }
                return phrases[indexer[currentIndex++]];
            }
            else if(generationMode == GenerationMode.Random)
            {
                return phrases[random.Next(phrases.Length)];
            }
            else return default;
        }

        public string GetText(int index) => default;

        public void SetRandom(System.Random random)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
            if (generationMode == GenerationMode.Cycle && indexer != null && indexer.Length > 0)
            {
                indexer.Shuffle(this.random);
                currentIndex = 0;
            }
        }
    }
}
