using UnityEngine;

[CreateAssetMenu(fileName = "RotationEffect", menuName = "TypTyp/Effects/RotationEffect")]
public class RotationEffect : StatusEffectDefinition
{
    [Range(0, 90)][SerializeField] float maxRotation;

    public override void OnActivate(Player target)
    {
        if (!target.IsServer) return;
        Transform rotTransform = Utils.FindChildrenWithTag(target.transform, "RotEffectPivot");
        float rot = Random.Range(-maxRotation, maxRotation);
        rotTransform.localRotation = Quaternion.identity;
        rotTransform.Rotate(Vector3.forward, rot, Space.Self);
    }

    public override void OnDeactivate(Player target) { }

    public override string GetDefaultValue()
    {
        return $"{maxRotation}";
    }
}
