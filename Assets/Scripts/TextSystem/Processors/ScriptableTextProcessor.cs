using TypTyp.TextSystem;
using UnityEngine;

namespace TypTyp.TextSystem
{
    public abstract class ScriptableTextProcessor : ScriptableObject, ITextProcessor
    {
        public abstract string ProcessText(string text);
    }
}