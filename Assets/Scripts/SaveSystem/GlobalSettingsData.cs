using System;
using TypTyp.Application;

[Serializable]
public class GlobalSettingsData
{
    public bool showSpaces = true;
    public bool chatActive = true;
    public bool ignoreCaseMenus = true;
    public bool capsLockWarning;
    public float volume = 0.75f;
    public int fontIndex;
    public bool initialTipUnderstood;
    public VideoSettingsData videoSettings = VideoSettingsData.CreateDefault();
}
