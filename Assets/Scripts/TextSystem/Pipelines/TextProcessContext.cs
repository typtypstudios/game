namespace TypTyp.TextSystem
{
    public readonly struct TextProcessContext
    {
        public readonly System.Random Random;

        public TextProcessContext(System.Random random)
        {
            Random = random;
        }
    }
}
