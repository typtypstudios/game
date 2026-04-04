using UnityEngine;

namespace TypTyp.TextSystem.Typable
{
    public abstract class TypableViewBase : MonoBehaviour, ITypableView
    {
        public abstract void UpdateView(in TypableViewDTO dto);
    }
}
