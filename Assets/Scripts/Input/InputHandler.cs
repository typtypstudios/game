using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
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

        private event Action<char> OnCharTyped; //Wraper, onTextInput no deja eliminar todos los listeners
        private event Action<char> OnCharTypedPriority;
        private readonly HashSet<Key> typedKeysHeld = new();
        private readonly HashSet<Key> keysConsumedThisFrame = new();
        private readonly Dictionary<char, Key> heldCharacterKeys = new();
        private int consumedFrame = -1;
        private Keyboard subscribedKeyboard;

        protected override void Awake()
        {
            base.Awake();
            OnCharTypedPriority = null;
            OnCharTyped = null;
            SyncKeyboardSubscription();
            SceneManager.sceneLoaded += (_, _) =>
            {
                Lag = 0;
                SetMode(InputModeMask.WaitingForPlayers);
            };
        }

        private void OnDestroy()
        {
            if (subscribedKeyboard != null)
                subscribedKeyboard.onTextInput -= ProcessInput;
        }

        private void Update()
        {
            SyncKeyboardSubscription();
            CleanupReleasedKeys();
        }

        private void SyncKeyboardSubscription()
        {
            Keyboard currentKeyboard = Keyboard.current;
            if (subscribedKeyboard == currentKeyboard)
                return;

            if (subscribedKeyboard != null)
                subscribedKeyboard.onTextInput -= ProcessInput;

            subscribedKeyboard = currentKeyboard;
            typedKeysHeld.Clear();
            heldCharacterKeys.Clear();

            if (subscribedKeyboard != null)
                subscribedKeyboard.onTextInput += ProcessInput;
        }

        private void ProcessInput(char c)
        {
            if (Keyboard.current != null && IsRepeatedHeldInput(c))
                return;

            if (Lag == 0) CommunicateChartTyped(c);
            else StartCoroutine(LagCoroutine(c));
        }

        private bool IsRepeatedHeldInput(char c)
        {
            CleanupReleasedKeys();
            ResetConsumedKeysIfNeeded();

            KeyControl key = ResolveInputKey(c);
            if (key != null)
            {
                Key keyCode = key.keyCode;
                if (typedKeysHeld.Contains(keyCode))
                    return true;

                typedKeysHeld.Add(keyCode);
                heldCharacterKeys[c] = keyCode;
                keysConsumedThisFrame.Add(keyCode);
                return false;
            }

            return heldCharacterKeys.TryGetValue(c, out Key heldKey) && IsKeyPressed(heldKey);
        }

        private KeyControl ResolveInputKey(char c)
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
                return null;

            KeyControl layoutKey = FindKeyOnCurrentLayout(c);
            if (layoutKey != null && layoutKey.wasPressedThisFrame && !keysConsumedThisFrame.Contains(layoutKey.keyCode))
                return layoutKey;

            foreach (KeyControl key in keyboard.allKeys)
            {
                if (!key.wasPressedThisFrame || keysConsumedThisFrame.Contains(key.keyCode) || IsModifierKey(key.keyCode))
                    continue;

                return key;
            }

            if (layoutKey != null && layoutKey.isPressed)
                return layoutKey;

            return null;
        }

        private KeyControl FindKeyOnCurrentLayout(char c)
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
                return null;

            if (c == ' ')
                return keyboard.spaceKey;

            return keyboard.FindKeyOnCurrentKeyboardLayout(c.ToString());
        }

        private void CleanupReleasedKeys()
        {
            if (Keyboard.current == null)
            {
                typedKeysHeld.Clear();
                heldCharacterKeys.Clear();
                return;
            }

            typedKeysHeld.RemoveWhere(key => !IsKeyPressed(key));

            List<char> charactersToRelease = null;
            foreach (KeyValuePair<char, Key> pair in heldCharacterKeys)
            {
                if (IsKeyPressed(pair.Value))
                    continue;

                charactersToRelease ??= new List<char>();
                charactersToRelease.Add(pair.Key);
            }

            if (charactersToRelease == null)
                return;

            foreach (char c in charactersToRelease)
                heldCharacterKeys.Remove(c);
        }

        private void ResetConsumedKeysIfNeeded()
        {
            if (consumedFrame == Time.frameCount)
                return;

            consumedFrame = Time.frameCount;
            keysConsumedThisFrame.Clear();
        }

        private bool IsKeyPressed(Key key)
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard[key].isPressed;
        }

        private static bool IsModifierKey(Key key)
        {
            return key == Key.LeftShift ||
                   key == Key.RightShift ||
                   key == Key.LeftCtrl ||
                   key == Key.RightCtrl ||
                   key == Key.LeftAlt ||
                   key == Key.RightAlt ||
                   key == Key.LeftMeta ||
                   key == Key.RightMeta ||
                   key == Key.CapsLock ||
                   key == Key.NumLock;
        }

        private void CommunicateChartTyped(char c)
        {
            if (char.IsControl(c)) return;
            OnCharTypedPriority?.Invoke(c);
            OnCharTyped?.Invoke(c);
        }

        public void AddListener(Action<char> func) => OnCharTyped += func;
        public void RemoveListener(Action<char> func) => OnCharTyped -= func;

        public void AddPriorityListener(Action<char> func) => OnCharTypedPriority += func;
        public void RemovePriorityListener(Action<char> func) => OnCharTypedPriority -= func;

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
