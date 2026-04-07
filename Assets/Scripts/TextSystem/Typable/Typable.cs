using System;

namespace TypTyp.TextSystem.Typable
{
    public class Typable
    {
        public int Idx { get; private set; }
        public string Text { get; private set; }
        public bool HasMistake { get; private set; } = false;
        public bool IsComplete { get; private set; } = false;

        TypableConfig config;

        public event Action OnChanged;
        public event Action OnError;
        public event Action OnComplete;

        public Typable(TypableConfig config)
        {
            this.config = config;
        }

        public void SetText(string text)
        {
            Text = text;
            Idx = 0;
            HasMistake = false;
            IsComplete = false;
            OnChanged?.Invoke();
        }

        public void Reset()
        {
            Idx = 0;
            HasMistake = false;
            IsComplete = false;
            OnChanged?.Invoke();
        }

        public void Input(char c)
        {
            if (string.IsNullOrEmpty(Text) || Idx >= Text.Length)
                return;

            char expected = Text[Idx];

            bool match = config.CaseSensitive
                ? c == expected
                : char.ToLower(c) == char.ToLower(expected);

            if (match)
            {
                HasMistake = false;
                Idx++;

                OnChanged?.Invoke();

                if (Idx >= Text.Length)
                {
                    IsComplete = true;
                    OnComplete?.Invoke();

                    if (config.ResetOnComplete)
                        Reset();
                }
            }
            else
            {
                HasMistake = true;
                OnError?.Invoke();

                if (config.ResetOnMistake)
                {
                    Reset();
                    return;
                }

                if (!config.StopOnMistake)
                {
                    Idx++;
                    OnChanged?.Invoke();
                }
                else if (config.MarkMistakes)
                {
                    OnChanged?.Invoke();
                }
            }
        }
    }
}
