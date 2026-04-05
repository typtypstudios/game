using System;
using UnityEngine;
using TypTyp.Input;

namespace TypTyp.TextSystem.Typable
{
    [RequireComponent(typeof(TypingInputListener))]
    public class TypableController : MonoBehaviour
    {
        [SerializeField] private TypableViewBase[] views;
        [SerializeField] private TypableConfigPreset configPreset;
        [SerializeField] private TypableConfig config = TypableConfig.Default;

        Typable typable;
        TypablePresenter presenter;
        TypingInputListener input; 

        public event Action OnComplete;
        public event Action OnChanged;
        public event Action OnError;

        public Func<char, char> InputTransform;

        public int Idx => typable != null ? typable.Idx : 0;
        public string Text => typable != null ? typable.Text : string.Empty;
        public bool HasMistake => typable != null && typable.HasMistake;

        void Awake()
        {
            input = GetComponent<TypingInputListener>();
            if (configPreset != null)
                config = configPreset.Config;
            typable = new Typable(config);
            presenter = new TypablePresenter(typable, views);
            typable.OnComplete += HandleComplete;
            typable.OnChanged += HandleChanged;
            typable.OnError += HandleError;
        }

        void Reset()
        {
            config = configPreset != null ? configPreset.Config : TypableConfig.Default;
        }

        void OnEnable()
        {
            input.OnInputTyped += HandleInput;
        }

        void OnDisable()
        {
            input.OnInputTyped -= HandleInput;
        }

        void OnDestroy()
        {
            if (typable != null)
            {
                typable.OnComplete -= HandleComplete;
                typable.OnChanged -= HandleChanged;
                typable.OnError -= HandleError;
            }
        }

        public void SetText(string text)
        {
            typable.SetText(text);
        }

        void HandleInput(char c)
        {
            char processed = InputTransform != null ? InputTransform(c) : c;
            typable.Input(processed);
        }

        void HandleComplete()
        {
            OnComplete?.Invoke();
        }

        void HandleChanged()
        {
            OnChanged?.Invoke();
        }

        void HandleError()
        {
            OnError?.Invoke();
        }
    }
}
