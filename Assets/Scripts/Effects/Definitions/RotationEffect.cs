using UnityEngine;

[CreateAssetMenu(fileName = "RotationEffect", menuName = "TypTyp/Effects/RotationEffect")]
public class RotationEffect : StatusEffectDefinition
{
    [Range(0, 90)][SerializeField] float maxRotation;

    public override void OnActivate(EffectContext context)
    {
        Player target = context.Target;
        System.Random rng = context.Random;
        if (rng == null)
            return;

        if (!target.IsServer) return;
        Transform rotTransform = GetRotationTransform(target);
        if (rotTransform == null)
            return;

        float rot = (float)(rng.NextDouble() * (maxRotation * 2f) - maxRotation);
        rotTransform.localRotation = Quaternion.identity;
        rotTransform.Rotate(Vector3.forward, rot, Space.Self);
    }

    public override void OnDeactivate(EffectContext context)
    {
        Player target = context.Target;
        if (!target.IsServer) return;

        Transform rotTransform = GetRotationTransform(target);
        if (rotTransform != null)
            rotTransform.localRotation = Quaternion.identity;
    }

    private Transform GetRotationTransform(Player target)
    {
        return Utils.FindChildrenWithTag(target.transform, "RotEffectPivot");
    }

    public override string GetDefaultValue()
    {
        return $"{maxRotation}";
    }
}

