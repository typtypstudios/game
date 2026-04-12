using UnityEngine;
using UnityEngine.InputSystem;
using TypTyp.Input;
using System;

[RequireComponent(typeof(Player))]
public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private InputActionReference changeModeActionReference;
    private Animator anim;
    [SerializeField] private Animator diverAnim;

    public Player Player { get; private set; }

    public event Action OnSilencedAttempt;
    public event Action OnBackspaceAttempt;
    private bool matchActive;
    public event Action<AnimState> OnAnimChanged;

    // Bool para el efecto de silenciado
    private bool isSilenced = false;

    void Awake()
    {
        MatchManager.OnMatchStarted += SubscribeToInput;
        MatchManager.OnMatchEnded += UnsubscribeToInput;
        anim = GetComponent<Animator>();
        Player = GetComponent<Player>();
    }

    void OnDestroy()
    {
        MatchManager.OnMatchStarted -= SubscribeToInput;
        MatchManager.OnMatchEnded -= UnsubscribeToInput;
        changeModeActionReference.action.started -= ChangeMode;
    }

    private void SubscribeToInput()
    {
        if (Player.IsOwner)
        {
            matchActive = true;
            Debug.Log("Subscribing to input for player " + Player.name, gameObject);
            changeModeActionReference.action.started += ChangeMode;
            SetMode(InputModeMask.Ritual);
        }
    }

    private void UnsubscribeToInput()
    {
        if (Player.IsOwner)
        {
            matchActive = false;
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
        diverAnim.SetBool("CastingSpells", mode == InputModeMask.Spells);
        if (Player.IsOwner) InputHandler.Instance.SetMode(mode);
    }

    private void Update()
    {
        if (!Player.IsOwner || !matchActive || Keyboard.current == null) return;

        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            OnBackspaceAttempt?.Invoke();
        }
    }

    public void OnAnimationChange()
    {
        OnAnimChanged?.Invoke(anim.GetBool("CastingSpells") ? AnimState.Spell : AnimState.Ritual);
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

public enum AnimState { Ritual, Spell };
