using UnityEngine;

namespace TypTyp.TextSystem
{
    [CreateAssetMenu(fileName = "SimplifyProcessor", menuName = "TypTyp/Text Processors/SimplifyProcessor")]
    public class SimplifyProcessor : ScriptableTextProcessor
    {
        public override string ProcessText(string input)
        {
            input = input.Replace(".", "").Replace(",", "");
            return input.ToLower();
        }
    }
}
