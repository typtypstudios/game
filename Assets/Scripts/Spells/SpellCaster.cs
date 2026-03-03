using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class SpellCaster : NetworkBehaviour
{
    Player player;
    MatchManager matchManager;

    public void Awake()
    {
        player = GetComponent<Player>();
        matchManager = FindAnyObjectByType<MatchManager>();
    }

    #region Server side

    public void CastSpell(SpellDefinition spellDef)
    {
        if (!IsServer) return;
        ulong casterId = OwnerClientId;
        int spellId = GetSpellId(spellDef);
        ulong targetId = 0;
        ApplySpell(OwnerClientId, spellId, targetId);
        ApplySpellRpc(OwnerClientId, spellId, targetId);
    }

    #endregion

    #region Client Side

    [Rpc(SendTo.ClientsAndHost)]
    public void ApplySpellRpc(ulong caster, int spell, params ulong[] targets)
    {
        ApplySpell(caster, spell, targets);
    }

    #endregion

    void ApplySpell(ulong caster, int spell, params ulong[] targets)
    {
        Player casterPlayer = GetPlayerById(caster);
        SpellDefinition spellDef = GetSpellById(spell);

        ApplyEffectsToPlayer(casterPlayer, spellDef.OnSelfEffects);

        foreach (var target in targets)
        {
            var targetPlayer = GetPlayerById(target);
            ApplyEffectsToPlayer(targetPlayer, spellDef.OnEnemyEffects);
        }
    }

    void ApplyEffectsToPlayer(Player player, IEnumerable<StatusEffectDefinition> effects)
    {
        if (player.TryGetComponent<StatusEffectController>(out var statusEffectController))
        {
            foreach (var effectDef in effects)
            {
                statusEffectController.AddEffect(effectDef);
            }
        }
    }

    #region Validation

    //De momento la validacion es la misma en server y en cliente
    public PlayCardRequestResult ValidateSpellCastRequest(RequestValidationType validationType, SpellDefinition spellDef)
    {
        Debug.LogFormat("Client ValidateSpellCastRequest\n PlayerMana: {0}\n SpellCost: {1}", player.CurrentMana.Value, spellDef.ManaCost);
        bool canCast = player.CurrentMana.Value >= spellDef.ManaCost;
        return canCast ? PlayCardRequestResult.Success : PlayCardRequestResult.NotEnoughMana;
    }

    #endregion

    #region Helper Methods

    Player GetPlayerById(ulong clientId) =>
        NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<Player>();

    SpellDefinition GetSpellById(int id) => SpellRegister.Instance.GetById(id);

    int GetSpellId(SpellDefinition spellDefinition) => SpellRegister.Instance.GetId(spellDefinition);

    #endregion
}
