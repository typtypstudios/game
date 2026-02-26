using System;
using UnityEngine;

namespace TypTyp.TextSystem
{
    public enum GenerationMode { None, Cycle, Random }

    public class RandomTextProvider : MonoBehaviour, ITextProvider
    {
        [SerializeField] TextAsset textSource;
        [SerializeField] int seed;
        [SerializeField] GenerationMode generationMode = GenerationMode.Cycle;
        [SerializeField, Min(1)] int repetitionsPerCycle = 1;

        [SerializeField]string[] phrases;
        System.Random random;

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

            random = new System.Random(seed);

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
    }
}
