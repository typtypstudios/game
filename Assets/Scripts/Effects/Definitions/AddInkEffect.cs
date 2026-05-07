using UnityEngine;

[CreateAssetMenu(fileName = "AddInkEffect", menuName = "TypTyp/Effects/AddInkEffect")]
public class AddInkEffect : StatusEffectDefinition
{
    [SerializeField] int tintBars;

    public override void OnActivate(EffectContext context)
    {
        Player target = context.Target;
        if (!target.IsServer) return;
        target.ManaManager.AddBars(tintBars);
    }

    public override void OnDeactivate(EffectContext context) { }

    public override string GetDefaultValue()
    {
        return $"{Mathf.Abs(tintBars)}";
    }
}
