using System.Collections.Generic;

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
