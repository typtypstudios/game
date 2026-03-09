using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private InputActionReference changeModeActionReference;
    private Animator anim;
    private RitualManager ritualManager;

    Player player;

    public event Action<InputMode> OnInputModeChangedEvent;

    void Awake()
    {
        MatchManager.OnMatchStarted += SubscribeToInput;
        MatchManager.OnMatchEnded += UnsubscribeToInput;
        anim = GetComponent<Animator>();
        ritualManager = GetComponentInChildren<RitualManager>(true);
        player = GetComponent<Player>();
    }

    void OnDestroy()
    {
        MatchManager.OnMatchStarted -= SubscribeToInput;
        MatchManager.OnMatchEnded -= UnsubscribeToInput;
        changeModeActionReference.action.performed -= ChangeMode;
    }

    private void SubscribeToInput() 
    {
        if (player.IsOwner)
        {
            changeModeActionReference.action.performed += ChangeMode;
            SetMode(InputMode.Ritual);
        }
    }

    private void UnsubscribeToInput()
    {
        if (player.IsOwner)
        {
            changeModeActionReference.action.performed -= ChangeMode;

            Debug.Log("Deshabilitando el input del ritual y los hechizos");
            SetMode(InputMode.GameEnded);
        }
    }

    private void ChangeMode(InputAction.CallbackContext ctx)
    {
        bool castingSpells = !anim.GetBool("CastingSpells");
        SetMode(castingSpells ? InputMode.CastingSpells : InputMode.Ritual);
    }

    private void SetMode(InputMode mode)
    {
        anim.SetBool("CastingSpells", mode == InputMode.CastingSpells);
        ritualManager.ToggleListener(mode == InputMode.Ritual);
        OnInputModeChangedEvent?.Invoke(mode);
    }
}

public enum InputMode
{
    Ritual,
    CastingSpells,
    GameEnded
}