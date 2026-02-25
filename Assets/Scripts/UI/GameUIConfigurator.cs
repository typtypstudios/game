using UnityEngine;

public class GameUIConfigurator : MonoBehaviour
{
    [Tooltip("Barras de progreso del ritual. La primera ha de ser la del cliente.")]
    [SerializeField] private UIBar[] ritualBars; 
    [Tooltip("Barras de man�. La primera ha de ser la del cliente.")]
    [SerializeField] private UIManaBar[] manaBars;
    [Tooltip("Barras de corrupci�n. La primera ha de ser la del cliente.")]
    [SerializeField] private UIBar[] corruptionBars;

    public void ConfigureUI()
    {
        Player.User.RitualProgress.OnValueChanged += ritualBars[0].UpdateValue;
        Player.Enemy.RitualProgress.OnValueChanged += ritualBars[1].UpdateValue;
        Player.User.CurrentMana.OnValueChanged += manaBars[0].UpdateValue;
        Player.Enemy.CurrentMana.OnValueChanged += manaBars[1].UpdateValue;
        foreach (UIManaBar bar in manaBars) bar.MaxValue = TypTyp.Settings.Instance.MaxMana;
        Player.User.CurrentCorruption.OnValueChanged += corruptionBars[0].UpdateValue;
        Player.Enemy.CurrentCorruption.OnValueChanged += corruptionBars[1].UpdateValue;
        foreach (UIBar bar in corruptionBars) bar.MaxValue = TypTyp.Settings.Instance.MaxCorruption;
    }
}