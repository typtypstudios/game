using System.Text;
using UnityEngine;

namespace TypTyp.TextSystem
{
    [CreateAssetMenu(fileName = "DoubleSpaceProcessor", menuName = "TypTyp/Text Processors/DoubleSpace")]
    public class DoubleSpaceProcessor : ScriptableTextProcessor
    {
        [SerializeField] int minSpacesAdded = 0;
        [SerializeField] int maxSpacesAdded = 2;

        public override string ProcessText(string input)
        {
            StringBuilder output = new();
            foreach(var c in input)
            {
                if (c == ' ')
                {
                    int spacesAdded = Random.Range(minSpacesAdded, maxSpacesAdded + 1) + 1; //El original se respeta con el +1
                    output.Append(' ', spacesAdded);
                }
                else output.Append(c);
            }
            return output.ToString();
        }
    }
}
