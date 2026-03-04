using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class SpellCaster : NetworkBehaviour
{
    Player player;
    ManaGainManager manaManager;

    public void Awake()
    {
        player = GetComponent<Player>();
        manaManager = GetComponent<ManaGainManager>();

        UnityEngine.Assertions.Assert.IsNotNull(player);
        UnityEngine.Assertions.Assert.IsNotNull(manaManager);
    }

    #region Server side

    public void CastSpell(SpellDefinition spellDef)
    {
        if (!IsServer) return;

        ulong casterId = OwnerClientId;
        int spellId = GetSpellId(spellDef);
        ulong targetId = 0;

        // Debug.Log($"[Spell][Server][Cast] caster={casterId} spell={spellId} manaBefore={player.CurrentMana.Value}");

        manaManager.ConsumeMana(spellDef.ManaCost);

        // Debug.Log($"[Spell][Server][ManaConsumed] caster={casterId} cost={spellDef.ManaCost} manaAfter={player.CurrentMana.Value}");

        //Dejo que se ejecute en todas las instancias, los efectos deciden que aplicar en server y en cliente
        // ApplySpell(casterId, spellId, targetId);

        // Debug.Log($"[Spell][Server][SendRPC] caster={casterId} spell={spellId} target={targetId}");

        ApplySpellRpc(casterId, spellId, targetId);
    }

    #endregion

    [Rpc(SendTo.Everyone)]
    public void ApplySpellRpc(ulong caster, int spell, params ulong[] targets)
    {
        // Debug.Log($"[Spell][RPC][Receive] localClient={NetworkManager.LocalClientId} caster={caster} spell={spell} targets={string.Join(",", targets)}");

        ApplySpell(caster, spell, targets);
    }

    void ApplySpell(ulong caster, int spell, params ulong[] targets)
    {
        // Debug.Log($"[Spell][Apply] caster={caster} spell={spell} targets={string.Join(",", targets)}");

        Player casterPlayer = GetPlayerById(caster);
        SpellDefinition spellDef = GetSpellById(spell);

        if (spellDef.OnSelfEffects.Any())
        {
            Debug.Log($"[Spell][ApplySelf] caster={caster} effects={spellDef.OnSelfEffects.Count()}");
            ApplyEffectsToPlayer(casterPlayer, spellDef.OnSelfEffects);
        }

        if (spellDef.OnEnemyEffects.Any())
        {
            foreach (var target in targets)
            {
                Debug.Log($"[Spell][ApplyTarget] caster={caster} target={target}");

                var targetPlayer = GetPlayerById(target);
                ApplyEffectsToPlayer(targetPlayer, spellDef.OnEnemyEffects);
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

    //De momento la validacion es la misma en server y en cliente
    public PlayCardRequestResult ValidateSpellCastRequest(RequestValidationType validationType, SpellDefinition spellDef)
    {
        // Debug.Log($"[Spell][Validate] type={validationType} cid={OwnerClientId} mana={player.CurrentMana.Value} cost={spellDef.ManaCost}");

        bool canCast = player.CurrentMana.Value >= spellDef.ManaCost;

        var res = canCast ? PlayCardRequestResult.Success : PlayCardRequestResult.NotEnoughMana;

        // Debug.Log($"[Spell][ValidateResult] type={validationType} cid={OwnerClientId} res={res}");

        return res;
    }

    #endregion

    #region Helper Methods

    Player GetPlayerById(ulong clientId) =>
        NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<Player>();

    SpellDefinition GetSpellById(int id) => SpellRegister.Instance.GetById(id);

    int GetSpellId(SpellDefinition spellDefinition) => SpellRegister.Instance.GetId(spellDefinition);

    #endregion
}
