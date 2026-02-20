using UnityEngine;

public abstract class AInputListener : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        InputHandler.Instance.AddListener(ProcessInput);
    }

    protected virtual void OnDisable()
    {
        InputHandler.Instance.RemoveListener(ProcessInput);
    }

    protected abstract void ProcessInput(char c);
}
