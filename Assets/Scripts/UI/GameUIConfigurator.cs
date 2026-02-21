using UnityEngine;

public class GameUIConfigurator : MonoBehaviour
{
    [Tooltip("Barras de progreso del ritual. La primera ha de ser la del cliente.")]
    [SerializeField] private UIBar[] ritualBars; 
    [Tooltip("Barras de maná. La primera ha de ser la del cliente.")]
    [SerializeField] private UIBar[] manaBars;
    [Tooltip("Barras de corrupción. La primera ha de ser la del cliente.")]
    [SerializeField] private UIBar[] corruptionBars;

    public void ConfigureUI()
    {
        Player.User.RitualProgress.OnValueChanged += ritualBars[0].UpdateValue;
        Player.Enemy.RitualProgress.OnValueChanged += ritualBars[1].UpdateValue;
        Player.User.CurrentMana.OnValueChanged += manaBars[0].UpdateValue;
        Player.Enemy.CurrentMana.OnValueChanged += manaBars[1].UpdateValue;
        foreach (UIBar bar in manaBars) bar.MaxValue = GlobalVariables.MaxMana;
        Player.User.CurrentCorruption.OnValueChanged += corruptionBars[0].UpdateValue;
        Player.Enemy.CurrentCorruption.OnValueChanged += corruptionBars[1].UpdateValue;
        foreach (UIBar bar in corruptionBars) bar.MaxValue = GlobalVariables.MaxCorruption;
    }
}