using UnityEngine;
using TypTyp.Input;

public abstract class AInputListener : MonoBehaviour
{
    [SerializeField] private Color fillColor = Color.white; //Color con el que se rellenan las letras
    [SerializeField] private bool autoSubscribe = true;
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
    public int Idx { get; protected set; } = 0;

    protected virtual void OnEnable()
    {
        fillColorTag = Utils.ColorToTag(fillColor);
        if(autoSubscribe) InputHandler.Instance.AddListener(ProcessInput);
    }

    protected virtual void OnDisable()
    {
        if(InputHandler.Instance != null) InputHandler.Instance.RemoveListener(ProcessInput);
    }

    public void ToggleListener(bool activate)
    {
        InputHandler.Instance.RemoveListener(ProcessInput); //En todo caso se borra para no suscribir 2 veces
        if (activate) InputHandler.Instance.AddListener(ProcessInput);
    }

    protected abstract void ProcessInput(char c);
}
