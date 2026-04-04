using UnityEngine;

namespace TypTyp.TextSystem.Typable
{
    public class TypableController : MonoBehaviour
    {
        [SerializeField] private TypableViewBase[] views;
        [SerializeField] TypableConfig config;

        Typable typable;
        TypablePresenter presenter;

        void Awake()
        {
            typable = new Typable(config);
            presenter = new TypablePresenter(typable, views);
            // if (views == null) return;
            // for (int i = 0; i < views.Length; i++)
            // {
            //     presenter.AddView(views[i]);
            // }
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
