using UnityEngine;

namespace TypTyp.TextSystem
{
    [CreateAssetMenu(fileName = "AlternatingCapsProcessor", menuName = "TypTyp/Text Processors/AlternatingCapsProcessor")]
    public class AlternatingCapsProcessor : ScriptableTextProcessor
    {
        public override string ProcessText(string input)
        {
            char[] chars = input.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (char.IsLetter(chars[i]))
                {
                    chars[i] = (i % 2 == 0) ? char.ToUpper(chars[i]) : char.ToLower(chars[i]);
                }
            }
            return new string(chars);
        }
    }
}