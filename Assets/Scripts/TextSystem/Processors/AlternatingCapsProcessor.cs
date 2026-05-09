using UnityEngine;
using System.Text;

namespace TypTyp.TextSystem
{
    [CreateAssetMenu(fileName = "AlternatingCapsProcessor", menuName = "TypTyp/Text Processors/AlternatingCaps")]
    public class AlternatingCapsProcessor : ScriptableTextProcessor
    {
        public override void ProcessText(StringBuilder builder, TextProcessContext context)
        {
            for (int i = 0; i < builder.Length; i++)
            {
                if (char.IsLetter(builder[i]))
                {
                    builder[i] = (i % 2 == 0) ? char.ToUpper(builder[i]) : char.ToLower(builder[i]);
                }
            }
        }
    }
}
