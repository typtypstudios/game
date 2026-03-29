using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class SpellCaster : NetworkBehaviour
{
    Player player;
    public bool Sealed { get; set; } = false;
    public event Action<CardDefinition, Player> OnSpellCasted;

    public void Awake()
    {
        player = GetComponent<Player>();

        UnityEngine.Assertions.Assert.IsNotNull(player);
    }

    //Called in all clients and server
    public void CastSpell(CardDefinition cardDef)
    {
        ulong targetId = NetworkManager.Singleton.ConnectedClientsIds.First(id => id != OwnerClientId);
        var target = GetPlayerById(targetId);
        if (!Sealed) ApplySpell(player, cardDef.Spell, target);
        else Sealed = false;
        OnSpellCasted?.Invoke(cardDef, player);
    }


    void ApplySpell(Player caster, SpellDefinition spellDef, params Player[] targets)
    {
        // Debug.Log($"[Spell][Apply] caster={caster} spell={spellDef} targets={string.Join(",", targets)}");

        if (spellDef.OnSelfEffects.Any())
        {
            Debug.Log($"[Spell][ApplySelf] caster={caster} effects={spellDef.OnSelfEffects.Count()}");
            ApplyEffectsToPlayer(caster, spellDef.OnSelfEffects);
        }

        if (spellDef.OnEnemyEffects.Any())
        {
            foreach (var target in targets)
            {
                Debug.Log($"[Spell][ApplyTarget] caster={caster} target={target}");
                ApplyEffectsToPlayer(target, spellDef.OnEnemyEffects);
            }
        }
    }

    void ApplyEffectsToPlayer(Player player, IEnumerable<StatusEffectDefinition> effects)
    {
        foreach (var effectDef in effects)
        {
            // Debug.Log($"[Spell][EffectApply] target={player.OwnerClientId} effect={effectDef.name}");
            player.StatusEffectController.AddEffect(effectDef);
        }
    }

    #region Validation

    // public PlayCardRequestResult ValidateSpellCastRequest(RequestValidationType validationType, CardDefinition cardDef)
    // {
    //     //Maybe will be used for delay validations
    //     return default;
    // }

    #endregion

    #region Helper Methods

    Player GetPlayerById(ulong clientId) =>
        NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<Player>();

    SpellDefinition GetSpellById(int id) => SpellRegister.Instance.GetById(id);

    int GetSpellId(SpellDefinition def) => SpellRegister.Instance.GetId(def);

    #endregion
}
