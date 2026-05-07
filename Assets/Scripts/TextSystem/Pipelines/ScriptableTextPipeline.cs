using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TypTyp.TextSystem
{
    [CreateAssetMenu(fileName = "ScriptableTextPipeline", menuName = "TypTyp/Text Pipelines/ScriptableTextPipeline")]
    public class ScriptableTextPipeline : ScriptableObject, ITextPipeline
    {
        [SerializeField] List<ScriptableTextProcessor> processors;
        private TextProcessContext context = new(new System.Random());

        public event Action<ITextProcessor> ProcessorAdded;
        public event Action<ITextProcessor> ProcessorRemoved;

        public string ProcessText(string text)
        {
            StringBuilder builder = new(text);
            foreach (var processor in processors)
            {
                processor.ProcessText(builder, context);
            }
            return builder.ToString();
        }

        public void SetContext(TextProcessContext context)
        {
            this.context = context;
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
