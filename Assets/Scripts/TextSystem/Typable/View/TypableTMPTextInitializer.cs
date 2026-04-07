using UnityEngine;
using TMPro;

namespace TypTyp.TextSystem.Typable
{
    public class TypableTMPTextInitializer : MonoBehaviour
    {
        [SerializeField] private TypableController controller;
        [SerializeField] private TMP_Text text;

        //Controller init in Awake
        private void Start()
        {
            if (controller == null || text == null) return;
            if (!string.IsNullOrEmpty(controller.Text)) return;
            controller.SetText(text.text ?? string.Empty);
        }
    }
}
