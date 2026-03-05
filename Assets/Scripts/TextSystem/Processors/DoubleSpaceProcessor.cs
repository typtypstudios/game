using UnityEngine;

namespace TypTyp.TextSystem
{
    [CreateAssetMenu(fileName = "DoubleSpaceProcessor", menuName = "TypTyp/Text Processors/DoubleSpaceProcessor")]
    public class DoubleSpaceProcessor : ScriptableTextProcessor
    {
        public override string ProcessText(string input)
        {
            return input.Replace(" ", "  ");
        }
    }
}
