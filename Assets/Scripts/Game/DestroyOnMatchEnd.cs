using UnityEngine;

public class DestroyOnMatchEnd : MonoBehaviour
{
    private void Awake()
    {
        MatchManager.OnMatchEnded += DestroySelf;
    }

    private void OnDestroy()
    {
        MatchManager.OnMatchEnded -= DestroySelf;
    }

    private void DestroySelf() => Destroy(this.gameObject);
}
