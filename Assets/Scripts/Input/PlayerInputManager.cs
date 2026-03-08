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
        MatchManager.OnMatchEnded += UnsubscribeToInput;
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
        MatchManager.OnMatchEnded -= UnsubscribeToInput;
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

    private void UnsubscribeToInput()
    {
        if (GetComponent<Player>().IsOwner)
        {
            changeModeActionReference.action.performed -= ChangeMode;

            Debug.Log("Deshabilitando el input del ritual y los hechizos");
            // Desactivar el input
            foreach (var card in cardUIs) card.ToggleListener(false);
            ritualManager.ToggleListener(false);
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