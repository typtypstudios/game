using System;

namespace TypTyp.TextSystem
{
    public readonly struct TextProcessContext
    {
        public readonly Random Random;

        public TextProcessContext(Random random)
        {
            Random = random;
        }
    }
}
