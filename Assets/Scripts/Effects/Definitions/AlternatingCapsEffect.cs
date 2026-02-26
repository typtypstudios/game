using TypTyp.TextSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "AlternatingCapsEffect", menuName = "TypTyp/Effects/AlternatingCapsEffect")]
public class AlternatingCapsEffect : StatusEffectDefinition
{
    [SerializeField] AlternatingCapsProcessor processor;

    public override void OnActivate(Player target)
    {
        if (!target.IsServer) return;
        if (target.TryGetComponent<ITextPipeline>(out var pipeline))
        {
            pipeline.AddProcessor(processor);
        }
    }

    public override void OnDeactivate(Player target)
    {
        if (!target.IsServer) return;
        if (target.TryGetComponent<ITextPipeline>(out var pipeline))
        {
            pipeline.RemoveProcessor(processor);
        }
    }
}