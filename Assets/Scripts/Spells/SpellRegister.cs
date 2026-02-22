using UnityEngine;

[CreateAssetMenu(fileName = "SpellRegister", menuName = "TypTyp/Spells/SpellRegister", order = 1)]
public class SpellRegister : ScriptableSingleton<SpellRegister>
{
    [SerializeField] private SpellData[] spells;

    void OEnable()
    {
        
    }

    public SpellData GetSpellById(int id)
    {
        if (id < 0 || id >= spells.Length)
        {
            Debug.LogError($"Invalid spell ID: {id}");
            return null;
        }
        return spells[id];
    }

    //En principio no debería ser necesario, pero por si acaso
    public SpellData GetSpellByName(string name)
    {
        foreach (var spell in spells)
        {
            if (spell.SpellName == name)
                return spell;
        }
        Debug.LogError($"Spell not found: {name}");
        return null;
    }

    public int GetSpellId(SpellData spell)
    {
        for (int i = 0; i < spells.Length; i++)
        {
            if (spells[i] == spell)
                return i;
        }
        Debug.LogError($"Spell not found in register: {spell.SpellName}");
        return -1;
    }
}
