using UnityEngine;

[NoAutoCreate]
[CreateAssetMenu(fileName = "SpellRegister", menuName = "TypTyp/Spells/SpellRegister", order = 1)]
public class SpellRegister : ScriptableSingleton<SpellRegister>
{
    [SerializeField] private SpellDefinition[] spells;

    public SpellDefinition GetSpellById(int id)
    {
        if (id < 0 || id >= spells.Length)
        {
            Debug.LogError($"Invalid spell ID: {id}");
            return null;
        }
        return spells[id];
    }

    //En principio no debería ser necesario, pero por si acaso
    public SpellDefinition GetSpellByName(string name)
    {
        foreach (var spell in spells)
        {
            if (spell.SpellName == name)
                return spell;
        }
        Debug.LogError($"Spell not found: {name}");
        return null;
    }

    public int GetSpellId(SpellDefinition spell)
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
/*
https://docs.unity3d.com/6000.3/Documentation/ScriptReference/ScriptableObject.html
Awake	    Called when an instance of ScriptableObject is created.
OnDestroy	This function is called when the scriptable object will be destroyed.
OnDisable	This function is called when the scriptable object goes out of scope.
OnEnable	This function is called when the object is loaded.
OnValidate	Editor-only function that Unity calls when the script is loaded or a value changes in the Inspector.
Reset	    Reset to default values.
*/