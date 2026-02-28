using UnityEngine;

public abstract class AInputListener : MonoBehaviour
{
    [SerializeField] private Color fillColor = Color.white; //Color con el que se rellenan las letras
    protected string fillColorTag; //string con el tag de ese color para TMPro
    public Color FillColor 
    { 
        get { return fillColor; } 
        set 
        { 
            fillColor = value;
            fillColorTag = Utils.ColorToTag(FillColor);
        } 
    }

    protected virtual void OnEnable()
    {
        fillColorTag = Utils.ColorToTag(fillColor);
        InputHandler.Instance.AddListener(ProcessInput);
    }

    protected virtual void OnDisable()
    {
        if(InputHandler.Instance != null) InputHandler.Instance.RemoveListener(ProcessInput);
    }

    public void ToggleListener(bool activate)
    {
        if(activate) InputHandler.Instance.AddListener(ProcessInput);
        else InputHandler.Instance.RemoveListener(ProcessInput);
    }

    protected abstract void ProcessInput(char c);
}
