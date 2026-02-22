using UnityEngine;

//Componente supuestamente en algun objeto de la escena que representa un hechizo activo, con su data y demas
public class Spell
{
    public SpellData Data { get; private set; }
    public SpellCaster SpellCaster { get; private set; }
    public Player[] Targets { get; private set; }

    public int GetSpellId() => SpellRegister.Instance.GetSpellId(Data);
}
