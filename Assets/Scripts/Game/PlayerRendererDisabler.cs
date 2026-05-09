using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PlayerRendererDisabler : MonoBehaviour
{
    private Renderer rend;
    private Player player;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        player = GetComponentInParent<Player>();
        if (player) player.OnPlayerConfigurated += ConfigureVisibility;
    }

    private void OnDestroy()
    {
        if (player) player.OnPlayerConfigurated -= ConfigureVisibility;
    }

    private void ConfigureVisibility()
    {
        if (player && player.IsOwner)
            rend.enabled = false;
    }
}
