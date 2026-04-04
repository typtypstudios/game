using UnityEngine;
using TypTyp.Input;

namespace TypTyp.TextSystem.Typable
{
    [RequireComponent(typeof(TypingInputListener))]
    public class TypableController : MonoBehaviour
    {
        [SerializeField] private TypableViewBase[] views;
        [SerializeField] TypableConfig config;

        Typable typable;
        TypablePresenter presenter;
        TypingInputListener input; 

        void Awake()
        {
            input = GetComponent<TypingInputListener>();
            typable = new Typable(config);
            presenter = new TypablePresenter(typable, views);
        }

        void OnEnable()
        {
            input.OnInputTyped += HandleInput;
        }

        void OnDisable()
        {
            input.OnInputTyped -= HandleInput;
        }

        public void SetText(string text)
        {
            typable.SetText(text);
        }

        void HandleInput(char c)
        {
            typable.Input(c);
        }
    }
}
