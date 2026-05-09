using System.Text;

namespace TypTyp.TextSystem
{
    public interface ITextProcessor
    {
        public void ProcessText(StringBuilder builder, TextProcessContext context);
    }
}
