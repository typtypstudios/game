using UnityEngine;
using TMPro;
using System.Text;

namespace TypTyp.TextSystem.Typable
{
    public class TMPTypableView : TypableViewBase
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private TypableViewStylePreset stylePreset;
        [SerializeField] private TypableViewStyleConfig styleConfig = TypableViewStyleConfig.Default;

        private bool wasComplete;
        private readonly StringBuilder sb = new();

        void Awake()
        {
            if (stylePreset != null)
                styleConfig = stylePreset.Config;
        }

        //Es posible que se pueda beneficiar de un string builder
        public override void UpdateView(in TypableViewDTO dto)
        {
            if (this.text == null) return;

            string safeText = dto.Text ?? "";
            int idx = dto.Idx;

            sb.Clear();

            sb.Append("<color=#");
            sb.Append(ColorUtility.ToHtmlStringRGB(styleConfig.CorrectColor));
            sb.Append('>');
            if (idx > 0)
                sb.Append(safeText, 0, idx);
            sb.Append("</color>");

            if (idx < safeText.Length)
            {
                char c = safeText[idx];
                if (dto.HasMistake)
                {
                    sb.Append("<color=#");
                    sb.Append(ColorUtility.ToHtmlStringRGB(styleConfig.WrongColor));
                    sb.Append('>');
                    sb.Append(c);
                    sb.Append("</color>");
                }
                else if (styleConfig.UnderlineNext)
                {
                    sb.Append("<u>");
                    sb.Append(c);
                    sb.Append("</u>");
                }
                else
                {
                    sb.Append(c);
                }
            }

            int restStart = idx + 1;
            if (restStart < safeText.Length)
                sb.Append(safeText, restStart, safeText.Length - restStart);

            this.text.text = sb.ToString();

            if (!wasComplete && dto.IsComplete && styleConfig.RandomizeCorrectColorOnComplete)
            {
                styleConfig.CorrectColor = Utils.GetDifferentColor(styleConfig.CorrectColor);
            }
            wasComplete = dto.IsComplete;
        }
    }
}
