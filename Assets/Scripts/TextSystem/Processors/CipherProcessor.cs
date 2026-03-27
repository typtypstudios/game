using System.Text;
using UnityEngine;

namespace TypTyp.TextSystem
{
    [CreateAssetMenu(fileName = "CipherProcessor", menuName = "TypTyp/Text Processors/CipherProcessor")]
    public class CipherProcessor : ScriptableTextProcessor
    {
        [Range(0, 1)][SerializeField] private float replaceProb = 0.25f;
        [SerializeField] private string specialChars;

        public override string ProcessText(string input)
        {
            StringBuilder sb = new();
            foreach(char c in input)
            {
                if (Random.Range(0f, 1f) < replaceProb) sb.Append(GetSpecialChar());
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private char GetSpecialChar()
        {
            int numChars = specialChars.Length;
            int idx = Random.Range(0, numChars);
            return specialChars[idx];
        }
    }
}
