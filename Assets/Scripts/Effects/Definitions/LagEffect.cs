using UnityEngine;
using TypTyp.Input;

[CreateAssetMenu(fileName = "LagEffect", menuName = "TypTyp/Effects/LagEffect")]
public class LagEffect : StatusEffectDefinition
{
    [Min(0)][SerializeField] float lagTime;

    public override void OnActivate(Player target)
    {
        if (target.IsOwner) InputHandler.Instance.Lag = lagTime;
    }

    public override void OnDeactivate(Player target) 
    {
        if (target.IsOwner) InputHandler.Instance.Lag = 0;
    }

    public override string GetDefaultValue()
    {
        return $"{lagTime}";
    }
}
