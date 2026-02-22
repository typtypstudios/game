using Unity.Netcode;
using UnityEngine;

//De momento voy a fingir que vivo en un mundo ideal y que los rpcs funcionan sin latencia
public class SpellCaster : NetworkBehaviour
{
    public bool TryCastSpell(Player caster, Spell spell)
    {
        if (!caster.IsOwner) return false;

        if(!CanCastSpell(caster, spell)) return false; //Tb se podria animacion de error por mana

        //Animacion de casteo previa a confirmacion de servidor

        // CastSpellRpc(spell.GetSpellId);
        return true;
    }

    [Rpc(SendTo.Server)]
    public void CastSpellRpc(int spellId, int casterId, params int[] targetsId)
    {
        if (!IsOwner) return; // Solo el dueño del objeto puede lanzar hechizos

        var spell = SpellRegister.Instance.GetSpellById(spellId);

        Debug.Log($"Player {OwnerClientId} casted spell: {spell.SpellName}");


    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ApplySpellRpc(int spellId, int casterId, params int[] targetsId)
    {
        var spell = SpellRegister.Instance.GetSpellById(spellId);
        //sacar los players por id
        // Player player = NetworkManager.Singleton.
        // spell.Cast(caster, targetsID)
    }


    bool CanCastSpell(Player caster, Spell spell)
    {
        return caster.CurrentMana.Value >= spell.Data.ManaCost;
    }
}
