using UnityEngine;

[CreateAssetMenu(fileName = "AddTintEffect", menuName = "TypTyp/Effects/AddTintEffect")]
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
        return $"{tintBars}";
    }
}