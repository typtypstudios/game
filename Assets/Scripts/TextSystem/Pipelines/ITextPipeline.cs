using System.Collections.Generic;

namespace TypTyp.TextSystem
{
    public interface ITextPipeline : ITextProcessor
    {
        public void AddProcessor(ITextProcessor processor);
        public void RemoveProcessor(ITextProcessor processor);
    }
}