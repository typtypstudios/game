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

        void Awake()
        {
            input = GetComponent<TypingInputListener>();
            if (configPreset != null)
                config = configPreset.Config;
            typable = new Typable(config);
            presenter = new TypablePresenter(typable, views);
            typable.OnComplete += HandleComplete;
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
                typable.OnComplete -= HandleComplete;
        }

        public void SetText(string text)
        {
            typable.SetText(text);
        }

        void HandleInput(char c)
        {
            typable.Input(c);
        }

        void HandleComplete()
        {
            OnComplete?.Invoke();
        }
    }
}
