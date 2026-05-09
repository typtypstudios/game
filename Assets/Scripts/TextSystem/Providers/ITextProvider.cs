namespace TypTyp.TextSystem
{
    public interface ITextProvider
    {
        public int Count { get; }
        public void SetRandom(System.Random random);
        public string GetText(int index);
        public string GetNextText();
    }
}
