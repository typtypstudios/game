using UnityEngine;
using System.Text;

namespace TypTyp.TextSystem
{
    [CreateAssetMenu(fileName = "SimplifyProcessor", menuName = "TypTyp/Text Processors/SimplifyProcessor")]
    public class SimplifyProcessor : ScriptableTextProcessor
    {
        [SerializeField] private string charsToRemove = ".!?:,";
        public override void ProcessText(StringBuilder builder, TextProcessContext context)
        {
            int writeIndex = 0;
            for (int readIndex = 0; readIndex < builder.Length; readIndex++)
            {
                char c = builder[readIndex];
                if (charsToRemove.Contains(c))
                    continue;

                builder[writeIndex++] = char.ToLower(c);
            }

            builder.Length = writeIndex;
        }
    }
}
