using System;
using UnityEditor.Callbacks;
using UnityEngine;

namespace TypTyp.Input
{
    [Flags]
    public enum InputModeMask : byte
    {
        Nothing = 0,
        Ritual = 1 << 0,
        Spells = 1 << 1,
        GameEnded = 1 << 2
    }


    public class TypingInputListener : MonoBehaviour, ITypingInputListener
    {
        [field: SerializeField] public InputModeMask InputMask = InputModeMask.Nothing;
        public event Action<char> OnInputTyped;

        void OnEnable()
        {
            InputHandler.Instance.AddListener(HandleInput);
        }

        void OnDisable()
        {
            InputHandler.Instance.RemoveListener(HandleInput);
        }

        void HandleInput(char c)
        {
            if (InputMask.HasFlag(GetGameInputMode()))
            {
                OnInputTyped?.Invoke(c);
            }
        }

        //TODO: Unificar esto con el PlayerInputManager, que ahora mismo usa el anterior enum
        InputModeMask GetGameInputMode()
        {
            return default;
        }
    }
}