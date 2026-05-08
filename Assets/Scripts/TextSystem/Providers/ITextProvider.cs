namespace TypTyp.TextSystem
{
    public interface ITextProvider
    {
        public void SetRandom(System.Random random);
        public string GetText(int index);
        public string GetNextText();
    }
}
