using System;
using UnityEngine;

public class GameUIConfigurator : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [Tooltip("Barras de progreso del ritual. La primera ha de ser la del cliente.")]
    [SerializeField] private UIBar[] ritualBars; 
    [Tooltip("Barras de man�. La primera ha de ser la del cliente.")]
    [SerializeField] private UIInkBar[] manaBars;
    [Tooltip("Barras de corrupci�n. La primera ha de ser la del cliente.")]
    [SerializeField] private UIBar[] corruptionBars;
    [Tooltip("UI de efectos de estados de cada jugador.")]
    [SerializeField] private StatusEffectUI[] statusEffectUIs;
    [Tooltip("Reminder text para los efectos")]
    [SerializeField] private PlayerFeedbackUI playerFeedbackUI;
    public static event Action OnUIConfigurated;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
    }

    public void ConfigureUI()
    {
        Player.User.RitualProgress.OnValueChanged += ritualBars[0].UpdateValue;
        Player.Enemy.RitualProgress.OnValueChanged += ritualBars[1].UpdateValue;
        Player.User.CurrentMana.OnValueChanged += manaBars[0].UpdateValue;
        Player.Enemy.CurrentMana.OnValueChanged += manaBars[1].UpdateValue;
        foreach (UIInkBar bar in manaBars) bar.MaxValue = TypTyp.Settings.Instance.MaxMana;
        Player.User.CurrentCorruption.OnValueChanged += corruptionBars[0].UpdateValue;
        Player.Enemy.CurrentCorruption.OnValueChanged += corruptionBars[1].UpdateValue;
        foreach (UIBar bar in corruptionBars) bar.MaxValue = TypTyp.Settings.Instance.MaxCorruption;
        statusEffectUIs[0].BindToPlayer(Player.User);
        statusEffectUIs[1].BindToPlayer(Player.Enemy);
        PlayerInputManager userInput = Player.User.GetComponent<PlayerInputManager>();
        if (userInput != null)
        {
            userInput.OnSilencedAttempt += playerFeedbackUI.ShowSilencedWarning;
        }
        if (Player.User.DeckController != null)
        {
            Player.User.DeckController.OnNotEnoughInkAttempt += playerFeedbackUI.ShowNotEnoughInkWarning;
        }
        canvasGroup.alpha = 1.0f;
        OnUIConfigurated?.Invoke();
    }
}