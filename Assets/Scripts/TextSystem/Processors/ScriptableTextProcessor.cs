using TypTyp.TextSystem;
using UnityEngine;
using System.Text;

namespace TypTyp.TextSystem
{
    public abstract class ScriptableTextProcessor : ScriptableObject, ITextProcessor
    {
        public abstract void ProcessText(StringBuilder builder, TextProcessContext context);
    }
}
