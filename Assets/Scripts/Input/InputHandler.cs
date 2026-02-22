using System;
using UnityEngine.InputSystem;

public class InputHandler : Singleton<InputHandler>
{
    private event Action<char> OnCharTyped; //Wraper, onTextInput no deja eliminar todos los listeners

    protected override void Awake()
    {
        base.Awake();
        Keyboard.current.onTextInput += (c) => OnCharTyped?.Invoke(c);
    }

    public void AddListener(Action<char> func) => OnCharTyped += func;

    public void RemoveListener(Action<char> func) => OnCharTyped -= func;

    /// <summary>
    /// Hace AddListener tras borrar la lista de listeners previa
    /// </summary>
    /// <param name="func"></param>
    public void SetUniqueListener(Action<char> func)
    {
        OnCharTyped = null;
        AddListener(func);
    }
}

