using UnityEngine;

public abstract class AAnimStateListener : MonoBehaviour
{
    private PlayerInputManager player;

    protected virtual void Awake()
    {
        player = GetComponentInParent<PlayerInputManager>();
        if (!player) Debug.LogError("Error: objeto fuera de la jerarquía de Player");
        player.OnAnimChanged += HandleStateChange;
        HandleStateChange(AnimState.Ritual); //Comienza en ritual
    }

    protected abstract void HandleStateChange(AnimState state);
}
