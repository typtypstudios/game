using UnityEngine;

[CreateAssetMenu(fileName = "AddInkEffect", menuName = "TypTyp/Effects/AddInkEffect")]
public class AddInkEffect : StatusEffectDefinition
{
    [SerializeField] int tintBars;

    public override void OnActivate(Player target)
    {
        if (!target.IsServer) return;
        target.ManaManager.AddBars(tintBars);
    }

    public override void OnDeactivate(Player target) { }

    public override string GetDefaultValue()
    {
        return $"{Mathf.Abs(tintBars)}";
    }
}