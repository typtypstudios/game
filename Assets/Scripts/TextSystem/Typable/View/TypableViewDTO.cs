namespace TypTyp.TextSystem.Typable
{
    public readonly struct TypableViewDTO
    {
        public readonly string Text;
        public readonly int Idx;
        public readonly bool HasMistake;
        public readonly bool IsComplete;

        public TypableViewDTO(string text, int idx, bool hasMistake, bool isComplete)
        {
            Text = text;
            Idx = idx;
            HasMistake = hasMistake;
            IsComplete = isComplete;
        }
    }
}
