using UnityEngine;

namespace TypTyp.TextSystem.Typable
{
    public class TypableController : MonoBehaviour
    {
        [SerializeField] TMPTypableView view;
        [SerializeField] TypableConfig config;

        Typable typable;
        TypablePresenter presenter;

        void Awake()
        {
            typable = new Typable(config);
            presenter = new TypablePresenter(typable);
            presenter.AddView(view);
        }

        void OnEnable()
        {
            InputHandler.Instance.AddListener(HandleInput);
        }

        void OnDisable()
        {
            InputHandler.Instance.RemoveListener(HandleInput);
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