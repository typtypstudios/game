namespace TypTyp.TextSystem.Typable
{
    [System.Serializable]
    public struct TypableConfig
    {
        public bool StopOnMistake;
        public bool MarkMistakes;
        public bool CaseSensitive;
        public bool ResetOnMistake;
        public bool ResetOnComplete;

        public static TypableConfig Default => new()
        {
            StopOnMistake = true,
            MarkMistakes = true,
            CaseSensitive = true,
            ResetOnMistake = false,
            ResetOnComplete = false
        };
    }
}
