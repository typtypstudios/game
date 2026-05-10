using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;

namespace TypTyp.TextSystem
{
    public class TextPipeline : MonoBehaviour, ITextPipeline
    {
        [SerializeField] List<ScriptableTextProcessor> processors;
        private readonly HashSet<ScriptableTextProcessor> runtimeProcessors = new();
        private readonly Dictionary<ScriptableTextProcessor, int> runtimeProcessorRefCounts = new();
        private TextProcessContext context;

        public IReadOnlyList<ITextProcessor> Processors => processors;

        public event Action<ITextProcessor> ProcessorAdded;
        public event Action<ITextProcessor> ProcessorRemoved;

        public string ProcessText(string text)
        {
            return ProcessText(text, null);
        }

        public string ProcessText(string text, Predicate<ITextProcessor> processorFilter)
        {
            if (context.Random == null)
            {
                Debug.LogError("TextPipeline context random not configured. Call SetContext before ProcessText.");
                return text;
            }

            StringBuilder builder = new(text);
            foreach (var processor in processors)
            {
                if (processorFilter != null && !processorFilter(processor))
                    continue;

                processor.ProcessText(builder, context);
            }
            return builder.ToString();
        }

        public void SetContext(TextProcessContext context)
        {
            this.context = context;
        }

        public bool IsRuntimeProcessor(ITextProcessor processor)
        {
            return processor is ScriptableTextProcessor scriptableProcessor &&
                runtimeProcessors.Contains(scriptableProcessor);
        }

        public void AddProcessor(ITextProcessor processor)
        {
            if (processor is not ScriptableTextProcessor scriptableProcessor)
                return;
            runtimeProcessorRefCounts.TryGetValue(scriptableProcessor, out int refCount);
            runtimeProcessorRefCounts[scriptableProcessor] = refCount + 1;

            if (!processors.Contains(scriptableProcessor))
            {
                processors.Add(scriptableProcessor);
                runtimeProcessors.Add(scriptableProcessor);
                ProcessorAdded?.Invoke(processor);
            }
        }

        public void RemoveProcessor(ITextProcessor processor)
        {
            if (processor is not ScriptableTextProcessor scriptableProcessor)
                return;

            if (!runtimeProcessorRefCounts.TryGetValue(scriptableProcessor, out int refCount))
                return;

            if (refCount > 1)
            {
                runtimeProcessorRefCounts[scriptableProcessor] = refCount - 1;
                return;
            }

            runtimeProcessorRefCounts.Remove(scriptableProcessor);
            if (runtimeProcessors.Remove(scriptableProcessor) && processors.Contains(scriptableProcessor))
            {
                processors.Remove(scriptableProcessor);
                ProcessorRemoved?.Invoke(processor);
            }
        }
    }
}
