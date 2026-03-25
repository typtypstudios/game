using System;
using System.Collections.Generic;

[Serializable]
public class GrimoireSection
{
    public string sectionName;
    public CardRegister cardRegister;
    public StatusEffectRegister effectRegister;
}

public struct GrimoirePage
{
    public int pageIndex;
    public int sectionIndex;
    public List<ADefinition> definitions;

    public GrimoirePage(int idx, int sectionIdx)
    {
        pageIndex = idx;
        sectionIndex = sectionIdx;
        definitions = new();
    }
}
