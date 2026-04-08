using UnityEngine;
using UnityEngine.InputSystem;
using TypTyp.Input;

[RequireComponent(typeof(Player))]
public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private InputActionReference changeModeActionReference;
    private Animator anim;

    Player player;

    public event System.Action OnSilencedAttempt;

    // Bool para el efecto de silenciado
    private bool isSilenced = false;

    void Awake()
    {
        MatchManager.OnMatchStarted += SubscribeToInput;
        MatchManager.OnMatchEnded += UnsubscribeToInput;
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    void OnDestroy()
    {
        MatchManager.OnMatchStarted -= SubscribeToInput;
        MatchManager.OnMatchEnded -= UnsubscribeToInput;
        changeModeActionReference.action.started -= ChangeMode;
    }

    private void SubscribeToInput() 
    {
        if (player.IsOwner)
        {
            Debug.Log("Subscribing to input for player " + player.name, gameObject);
            changeModeActionReference.action.started += ChangeMode;
            SetMode(InputModeMask.Ritual);
        }
    }

    private void UnsubscribeToInput()
    {
        if (player.IsOwner)
        {
            changeModeActionReference.action.started -= ChangeMode;
            SetMode(InputModeMask.GameEnded);
        }
    }

    private void ChangeMode(InputAction.CallbackContext ctx)
    {
        if (isSilenced)
        {
            OnSilencedAttempt?.Invoke();
            return;
        }
        ChangeModeLogic();
    }

    private void ChangeModeLogic()
    {
        bool castingSpells = !anim.GetBool("CastingSpells");
        SetMode(castingSpells ? InputModeMask.Spells : InputModeMask.Ritual);
    }

    private void SetMode(InputModeMask mode)
    {
        anim.SetBool("CastingSpells", mode == InputModeMask.Spells);
        InputHandler.Instance.SetMode(mode);
    }

    #region SpellEffects

    public void SilenceSpellEffect(bool state)
    {
        isSilenced = state;
        if (state) SetMode(InputModeMask.Ritual);
    }
    public void SwapEffect()
    {
        // Si el jugador esta silenciado: no hacer nada
        if (isSilenced) return;
        ChangeModeLogic();
    }

    #endregion
}
