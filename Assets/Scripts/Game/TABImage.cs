using UnityEngine;

public class TABImage : AAnimStateListener
{
    protected override void HandleStateChange(AnimState state)
    {
        Vector3 scale = Vector3.one;
        scale.x = state == AnimState.Ritual ? 1.0f : -1.0f;
        transform.localScale = scale;
    }
}
