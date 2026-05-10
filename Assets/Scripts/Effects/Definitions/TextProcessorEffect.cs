using TypTyp.TextSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "TextProcessorEffect", menuName = "TypTyp/Effects/TextProcessorEffect")]
public class TextProcessorEffect : StatusEffectDefinition
{
    [SerializeField] ScriptableTextProcessor processor;

    public ScriptableTextProcessor Processor => processor;

    public override void OnActivate(EffectContext context)
    {
        Player target = context.Target;
        // if (!target.IsServer) return;
        if (target.TryGetComponent<ITextPipeline>(out var pipeline))
        {
            pipeline.AddProcessor(processor);
        }
    }

    public override void OnDeactivate(EffectContext context)
    {
        Player target = context.Target;
        // if (!target.IsServer) return;
        if (target.TryGetComponent<ITextPipeline>(out var pipeline))
        {
            pipeline.RemoveProcessor(processor);
        }
    }

    public override string GetDefaultValue()
    {
        return "";
    }
}
