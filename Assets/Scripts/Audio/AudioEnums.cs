public enum MusicTrack
{
    None = 0,
    MainMenu,
    Match,
    GameEnd
}

public enum CountdownSound
{
    None = 0,
    Three,
    Two,
    One,
    Go
}

public enum UISound
{
    None = 0,
    ButtonClick,
    ExchangeCards,
    SelectCard,
    ChangeCult,
    FlipPage
}

public enum GameSound
{
    None = 0,
    SpellCast,
    EnemySpell,
    FailRitual, // No se usa, el sonido de tos está en RitualChoirPlayer en el MatchSceneMusicAudio
    Damage,
    Heal
}

public enum CultSound
{
    None = -1,
    Octopus = 0,
    Granny = 1,
    XCel = 2
}