using System.Text;
using UnityEngine;

namespace TypTyp.TextSystem
{
    [CreateAssetMenu(fileName = "DoubleSpaceProcessor", menuName = "TypTyp/Text Processors/DoubleSpace")]
    public class DoubleSpaceProcessor : ScriptableTextProcessor
    {
        [SerializeField] int minSpacesAdded = 0;
        [SerializeField] int maxSpacesAdded = 2;

        public override void ProcessText(StringBuilder builder, TextProcessContext context)
        {
            if (builder.Length == 0)
                return;

            StringBuilder output = new(builder.Length);
            for (int i = 0; i < builder.Length; i++)
            {
                char c = builder[i];
                if (c == ' ')
                {
                    int spacesAdded = Random.Range(minSpacesAdded, maxSpacesAdded + 1) + 1; //El original se respeta con el +1
                    output.Append(' ', spacesAdded);
                }
                else output.Append(c);
            }

            builder.Clear();
            builder.Append(output);
        }
    }
}
