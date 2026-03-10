using System.Text;
using UnityEngine;

namespace TypTyp.TextSystem
{
    [CreateAssetMenu(fileName = "CamelCaseProcessor", menuName = "TypTyp/Text Processors/CamelCase")]
    public class CamelCaseProcessor : ScriptableTextProcessor
    {
        public override string ProcessText(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var sb = new StringBuilder(input.Length);
            bool capitalizeNext = false;
            bool firstWord = true;

            int spaceCounter = 0;
            int nextSpaceToKeep = UnityEngine.Random.Range(2, 5); // 2–4

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (char.IsWhiteSpace(c))
                {
                    capitalizeNext = !firstWord;

                    spaceCounter++;

                    if (spaceCounter >= nextSpaceToKeep)
                    {
                        sb.Append(' ');
                        spaceCounter = 0;
                        nextSpaceToKeep = UnityEngine.Random.Range(2, 5);
                    }

                    continue;
                }

                if (firstWord)
                {
                    sb.Append(char.ToLowerInvariant(c));
                    firstWord = false;
                }
                else if (capitalizeNext)
                {
                    sb.Append(char.ToUpperInvariant(c));
                    capitalizeNext = false;
                }
                else
                {
                    sb.Append(char.ToLowerInvariant(c));
                }
            }

            return sb.ToString();
        }
    }
}
