using UnityEngine;

public abstract class AInputListener : MonoBehaviour
{
    [SerializeField] private Color fillColor = Color.white;
    protected string fillColorTag;

    protected virtual void OnEnable()
    {
        fillColorTag = $"<color #{ColorUtility.ToHtmlStringRGB(fillColor)}>";
        InputHandler.Instance.AddListener(ProcessInput);
    }

    protected virtual void OnDisable()
    {
        InputHandler.Instance.RemoveListener(ProcessInput);
    }

    protected abstract void ProcessInput(char c);
}
