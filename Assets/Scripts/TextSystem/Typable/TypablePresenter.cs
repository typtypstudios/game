namespace TypTyp.TextSystem.Typable
{
    using System;
    using System.Collections.Generic;

    public class TypablePresenter
    {
        private readonly Typable typable;
        private readonly List<ITypableView> views;

        public TypablePresenter(Typable typable)
            : this(typable, (IEnumerable<ITypableView>)null)
        {
        }

        public TypablePresenter(Typable typable, ITypableView view)
            : this(typable, view != null ? new[] { view } : Array.Empty<ITypableView>())
        {
        }

        public TypablePresenter(Typable typable, IEnumerable<ITypableView> views)
        {
            this.typable = typable;
            this.views = views != null ? new List<ITypableView>(views) : new List<ITypableView>();

            typable.OnChanged += OnChanged;
        }

        public void AddView(ITypableView view)
        {
            if (view == null) return;
            if (views.Contains(view)) return;

            views.Add(view);
            TypableViewDTO dto = BuildDTO();
            view.UpdateView(in dto);
        }

        public void RemoveView(ITypableView view)
        {
            if (view == null) return;
            views.Remove(view);
        }

        private void OnChanged()
        {
            TypableViewDTO dto = BuildDTO();
            for (int i = 0; i < views.Count; i++)
            {
                views[i].UpdateView(in dto);
            }
        }

        private TypableViewDTO BuildDTO()
        {
            return new TypableViewDTO(typable.Text, typable.Idx, typable.HasMistake, typable.IsComplete);
        }
    }
}
