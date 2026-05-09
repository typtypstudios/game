public readonly struct EffectContext
{
    public readonly Player Target;
    public readonly System.Random Random;

    public EffectContext(Player target, System.Random random)
    {
        Target = target;
        Random = random;
    }
}
