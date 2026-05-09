using System;

namespace TypTyp.TextSystem
{
    public interface ITextPipeline
    {
        public string ProcessText(string text);
        public void SetContext(TextProcessContext context);
        public void AddProcessor(ITextProcessor processor);
        public void RemoveProcessor(ITextProcessor processor);

        public event Action<ITextProcessor> ProcessorAdded;
        public event Action<ITextProcessor> ProcessorRemoved;
    }
}
