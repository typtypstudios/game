using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TypTyp.Input
{
    [Flags]
    public enum InputModeMask : byte
    {
        Nothing = 0,
        Ritual = 1 << 0,
        Spells = 1 << 1,
        GameEnded = 1 << 2,
        WaitingForPlayers = 1 << 3,
    }

    [NoAutoCreate]
    public class InputHandler : Singleton<InputHandler>
    {
        public float Lag { get; set; } = 0;
        [field: SerializeField] public InputModeMask CurrentMode { get; private set; } = ~InputModeMask.Nothing;

        public event Action<InputModeMask> OnInputModeChanged;
        public event Action OnBackspaceDuringGame;

        private event Action<char> OnCharTyped; //Wraper, onTextInput no deja eliminar todos los listeners

        protected override void Awake()
        {
            base.Awake();
            OnCharTyped = null;
            Keyboard.current.onTextInput += ProcessInput;
            SceneManager.sceneLoaded += (_, _) =>
            {
                Lag = 0;
                SetMode(InputModeMask.WaitingForPlayers);
            };
        }

        private void OnDestroy() => Keyboard.current.onTextInput -= ProcessInput;

        private void ProcessInput(char c)
        {
            // Si está en partida, mostrar que el backspace no sirve para nada
            if (c == '\b' && (CurrentMode == InputModeMask.Ritual || CurrentMode == InputModeMask.Spells))
                OnBackspaceDuringGame?.Invoke();

            if (Lag == 0) CommunicateChartTyped(c);
            else StartCoroutine(LagCoroutine(c));
        }

        private void CommunicateChartTyped(char c)
        {
            if (!char.IsControl(c)) OnCharTyped?.Invoke(c);
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

        public void SetMode(InputModeMask mode)
        {
            if (CurrentMode == mode) return;
            CurrentMode = mode;
            OnInputModeChanged?.Invoke(mode);
        }

        IEnumerator LagCoroutine(char c)
        {
            yield return new WaitForSeconds(Lag);
            CommunicateChartTyped(c);
        }
    }
}
