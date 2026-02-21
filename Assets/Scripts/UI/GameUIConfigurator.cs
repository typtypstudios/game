using UnityEngine;

public class GameUIConfigurator : MonoBehaviour
{
    [Tooltip("Barras de progreso del ritual. La primera ha de ser la del cliente.")]
    [SerializeField] private UIBar[] ritualBars; 
    [Tooltip("Barras de maná. La primera ha de ser la del cliente.")]
    [SerializeField] private UIBar[] manaBars;

    public void ConfigureUI()
    {
        Player.User.RitualProgress.OnValueChanged += ritualBars[0].UpdateValue;
        Player.Enemy.RitualProgress.OnValueChanged += ritualBars[1].UpdateValue;
        Player.User.CurrentMana.OnValueChanged += manaBars[0].UpdateValue;
        Player.Enemy.CurrentMana.OnValueChanged += manaBars[1].UpdateValue;
    }
}