using UnityEngine;
using TMPro;
using System.Text;

namespace TypTyp.TextSystem.Typable
{
    public class TMPTypableView : TypableViewBase
    {
        [SerializeField] private TMP_Text tmp;
        [SerializeField] private TypableViewStylePreset stylePreset;
        [SerializeField] public TypableViewStyleConfig StyleConfig = TypableViewStyleConfig.Default;

        private bool wasComplete;
        private readonly StringBuilder sb = new();

        void Awake()
        {
            if (stylePreset != null)
                StyleConfig = stylePreset.Config;
        }

        public override void UpdateView(in TypableViewDTO dto)
        {
            if (tmp == null) return;

            string safeText = dto.Text ?? "";
            int idx = dto.Idx;

            sb.Clear();

            sb.Append("<color=#");
            sb.Append(ColorUtility.ToHtmlStringRGB(StyleConfig.CorrectColor));
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
                    sb.Append(ColorUtility.ToHtmlStringRGB(StyleConfig.WrongColor));
                    sb.Append('>');
                    sb.Append(c);
                    sb.Append("</color>");
                }
                else if (StyleConfig.UnderlineNext)
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

            string finalText = sb.ToString();
            if (Settings.Instance.ShowSpaces && StyleConfig.isAbleToShowSpaces)
                finalText = finalText.Replace(" ", Settings.Instance.SpaceReplacement);

            tmp.text = finalText;

            if (!wasComplete && dto.IsComplete && StyleConfig.RandomizeCorrectColorOnComplete)
            {
                StyleConfig = new TypableViewStyleConfig
                {
                    CorrectColor = Utils.GetDifferentColor(StyleConfig.CorrectColor),
                    WrongColor = StyleConfig.WrongColor,
                    UnderlineNext = StyleConfig.UnderlineNext,
                    RandomizeCorrectColorOnComplete = StyleConfig.RandomizeCorrectColorOnComplete
                };
            }
            wasComplete = dto.IsComplete;
        }
    }
}
