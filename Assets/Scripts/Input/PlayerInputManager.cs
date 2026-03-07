using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private InputActionReference changeModeActionReference;
    private Animator anim;
    private RitualManager ritualManager;
    private WritableSpell[] cardUIs;

    void Awake()
    {
        MatchManager.OnMatchStarted += SubscribeToInput;
        anim = GetComponent<Animator>();
        ritualManager = GetComponentInChildren<RitualManager>(true);
    }

    private void Start()
    {
        cardUIs = GetComponentsInChildren<WritableSpell>(true); //Se crean en Awake de CardUIManager
    }

    void OnDestroy()
    {
        MatchManager.OnMatchStarted -= SubscribeToInput;
        changeModeActionReference.action.performed -= ChangeMode;
    }

    private void SubscribeToInput() 
    {
        if (GetComponent<Player>().IsOwner)
        {
            changeModeActionReference.action.performed += ChangeMode;
            SetMode(InputMode.Ritual);
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
        foreach (var card in cardUIs) card.ToggleListener(mode == InputMode.CastingSpells);
    }
}

enum InputMode
{
    Ritual,
    CastingSpells
}