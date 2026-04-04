using UnityEngine;
using UnityEngine.UI;

namespace TypTyp.TextSystem.Typable
{
    public class TypableProgressBarView : TypableViewBase
    {
        [SerializeField] private Image fill;

        public override void UpdateView(in TypableViewDTO dto)
        {
            if (fill == null) return;

            int length = string.IsNullOrEmpty(dto.Text) ? 0 : dto.Text.Length;
            if (length == 0)
            {
                fill.fillAmount = 0f;
                return;
            }

            int idx = dto.Idx;
            if (idx < 0) idx = 0;
            if (idx > length) idx = length;
            fill.fillAmount = (float)idx / length;
        }
    }
}
