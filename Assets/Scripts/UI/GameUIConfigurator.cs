using UnityEngine;

public class GameUIConfigurator : MonoBehaviour
{
    [Tooltip("Barras de progreso del ritual. La primera ha de ser la del cliente.")]
    [SerializeField] private UIBar[] ritualBars;

    public void ConfigureUI()
    {
        Player.User.RitualProgress.OnValueChanged += ritualBars[0].UpdateValue;
        Player.Enemy.RitualProgress.OnValueChanged += ritualBars[1].UpdateValue;
    }
}