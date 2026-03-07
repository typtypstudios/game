using System.Collections.Generic;
using UnityEngine;

namespace TypTyp.TextSystem
{
    [CreateAssetMenu(fileName = "ScriptableTextPipeline", menuName = "TypTyp/Text Pipelines/ScriptableTextPipeline")]
    public class ScriptableTextPipeline : ScriptableObject, ITextPipeline
    {
        [SerializeField] List<ScriptableTextProcessor> processors;

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
            }
        }

        public void RemoveProcessor(ITextProcessor processor)
        {
            if (processor is not ScriptableTextProcessor scriptableProcessor)
                return;
            if (processors.Contains(scriptableProcessor))
            {
                processors.Remove(scriptableProcessor);
            }
        }
    }
}