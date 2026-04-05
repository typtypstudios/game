using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int version = 1;
    public ProfileSaveData profile = new();
    public int cultId = 0;
    public CultData[] cultData;
}

[Serializable]
public class SaveState
{
    public SaveData slot = new();
    public GlobalSettingsData global = new();
}

[Serializable]
public class ProfileSaveData
{
    public string username = string.Empty;
}

[Serializable]
public class DeckSaveData
{
    public List<int> equippedCardIds = new();
}

[Serializable] 
public class CultData
{
    public int level;
    public DeckSaveData deck = new();
}
