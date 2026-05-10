using System;
using System.Collections.Generic;

namespace TypTyp.TextSystem
{
    public interface ITextPipeline
    {
        public IReadOnlyList<ITextProcessor> Processors { get; }
        public string ProcessText(string text);
        public string ProcessText(string text, Predicate<ITextProcessor> processorFilter);
        public bool IsRuntimeProcessor(ITextProcessor processor);
        public void SetContext(TextProcessContext context);
        public void AddProcessor(ITextProcessor processor);
        public void RemoveProcessor(ITextProcessor processor);

        public event Action<ITextProcessor> ProcessorAdded;
        public event Action<ITextProcessor> ProcessorRemoved;
    }
}
