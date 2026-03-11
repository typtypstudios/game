using UnityEngine;
using System.Collections.Generic;
using System;

namespace TypTyp.TextSystem
{
    public class TextPipeline : MonoBehaviour, ITextPipeline
    {
        [SerializeField] List<ScriptableTextProcessor> processors;

        public event Action<ITextProcessor> ProcessorAdded;
        public event Action<ITextProcessor> ProcessorRemoved;

        public string ProcessText(string text)
        {
            string processedText = text;
            foreach (var processor in processors)
            {
                processedText = processor.ProcessText(processedText);
            }
            return processedText;
        }

        public void AddProcessor(ITextProcessor processor)
        {
            if (processor is not ScriptableTextProcessor scriptableProcessor)
                return;
            if (!processors.Contains(scriptableProcessor))
            {
                processors.Add(scriptableProcessor);
                ProcessorAdded?.Invoke(processor);
            }
        }

        public void RemoveProcessor(ITextProcessor processor)
        {
            if (processor is not ScriptableTextProcessor scriptableProcessor)
                return;
            if (processors.Contains(scriptableProcessor))
            {
                processors.Remove(scriptableProcessor);
                ProcessorRemoved?.Invoke(processor);
            }
        }
    }
}