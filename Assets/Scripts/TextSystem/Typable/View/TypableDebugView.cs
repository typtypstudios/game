using UnityEngine;

namespace TypTyp.TextSystem.Typable
{
    public class TypableDebugView : MonoBehaviour, ITypableView
    {
        public void UpdateView(in TypableViewDTO dto)
        {
            int length = string.IsNullOrEmpty(dto.Text) ? 0 : dto.Text.Length;
            Debug.Log($"[{dto.Idx}/{length}] Error: {dto.HasMistake} Complete: {dto.IsComplete}");
        }
    }
}
