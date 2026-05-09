using System.Text;
using UnityEngine;
using System;

namespace TypTyp.TextSystem
{
    [CreateAssetMenu(fileName = "CipherProcessor", menuName = "TypTyp/Text Processors/CipherProcessor")]
    public class CipherProcessor : ScriptableTextProcessor
    {
        [Range(0, 1)][SerializeField] private float replaceProb = 0.25f;
        [SerializeField] private string specialChars;

        public override void ProcessText(StringBuilder builder, TextProcessContext context)
        {
            if (string.IsNullOrEmpty(specialChars))
                return;

            System.Random rng = context.Random;
            if (rng == null)
                return;

            for (int i = 0; i < builder.Length; i++)
            {
                if (rng.NextDouble() < replaceProb)
                    builder[i] = GetSpecialChar(rng);
            }
        }

        private char GetSpecialChar(System.Random rng)
        {
            int idx = rng.Next(0, specialChars.Length);
            return specialChars[idx];
        }
    }
}
