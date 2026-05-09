using System;

public class MatchRandomContext
{
    public readonly Random Ritual; //Only Server
    public readonly Random Deck; //only Server
    public readonly Random Spells; //Client and server, since clients execute spells

    public MatchRandomContext(int seed)
    {
        Ritual = new(seed + 1);
        Deck = new(seed + 2);
        Spells = new(seed + 3);
    }
}