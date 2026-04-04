using System;
using UnityEngine;

namespace TypTyp.Input
{
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
            if (InputMask.HasFlag(InputHandler.Instance.CurrentMode))
            {
                OnInputTyped?.Invoke(c);
            }
        }
    }
}
