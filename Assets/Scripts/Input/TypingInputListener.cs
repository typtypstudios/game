using System;
using UnityEngine;

namespace TypTyp.Input
{
    public class TypingInputListener : MonoBehaviour, ITypingInputListener
    {
        [field: SerializeField] public InputModeMask InputMask = ~InputModeMask.Nothing;
        public event Action<char> OnInputTyped;

        void OnEnable()
        {
            InputHandler.Instance.AddListener(HandleInput);
        }

        void OnDisable()
        {
            if (InputHandler.Instance != null)
                InputHandler.Instance.RemoveListener(HandleInput);
        }

        void HandleInput(char c)
        {
            if ((InputMask & InputHandler.Instance.CurrentMode) != 0)
            {
                OnInputTyped?.Invoke(c);
            }
        }
    }
}
