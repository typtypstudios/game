using UnityEngine;

[CreateAssetMenu(fileName = "MistakeHarvestEffect", menuName = "TypTyp/Effects/MistakeHarvestEffect")]
public class MistakeHarvestEffect : StatusEffectDefinition
{
    public override void OnActivate(Player target)
    {
        if (!target.IsServer) return;
        target.CorruptionManager.OnMistake += ProcessMistake;
    }

    public override void OnDeactivate(Player target)
    {
        if (!target.IsServer) return;
        target.CorruptionManager.OnMistake -= ProcessMistake;
    }

    public override string GetDefaultValue()
    {
        return "";
    }

    private void ProcessMistake(float corruption, Player target)
    {
        Player healingTarget = target == Player.User ? Player.Enemy : Player.User;
        healingTarget.CorruptionManager.AddCorruption(-corruption);
    }
}