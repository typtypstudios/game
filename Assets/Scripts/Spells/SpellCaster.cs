using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class SpellCaster : NetworkBehaviour
{
    Player player;
    ManaGainManager manaManager;
    public static event Action<ulong, CardDefinition> OnCardApplied;

    public void Awake()
    {
        player = GetComponent<Player>();
        manaManager = GetComponent<ManaGainManager>();

        UnityEngine.Assertions.Assert.IsNotNull(player);
        UnityEngine.Assertions.Assert.IsNotNull(manaManager);
    }

    #region Server side

    public void CastSpell(CardDefinition cardDef)
    {
        if (!IsServer) return;

        ulong casterId = OwnerClientId;
        int cardId = GetCardId(cardDef);
        //De momento, si solo hay dos jugadores, elijo al otro
        ulong targetId = NetworkManager.Singleton.ConnectedClientsIds.First(id => id != casterId);

        // Debug.Log($"[Spell][Server][Cast] caster={casterId} spell={spellId} manaBefore={player.CurrentMana.Value}");

        manaManager.ConsumeMana(cardDef.ManaCost);

        // Debug.Log($"[Spell][Server][ManaConsumed] caster={casterId} cost={spellDef.ManaCost} manaAfter={player.CurrentMana.Value}");

        //Dejo que se ejecute en todas las instancias, los efectos deciden que aplicar en server y en cliente
        // ApplySpell(casterId, spellId, targetId);

        // Debug.Log($"[Spell][Server][SendRPC] caster={casterId} spell={spellId} target={targetId}");

        ApplySpellRpc(casterId, cardId, targetId);
    }

    #endregion

    [Rpc(SendTo.Everyone)]
    public void ApplySpellRpc(ulong caster, int cardId, params ulong[] targets)
    {
        // Debug.Log($"[Spell][RPC][Receive] localClient={NetworkManager.LocalClientId} caster={caster} spell={spell} targets={string.Join(",", targets)}");

        ApplySpell(caster, cardId, targets);
    }

    void ApplySpell(ulong caster, int cardId, params ulong[] targets)
    {
        // Debug.Log($"[Spell][Apply] caster={caster} spell={spell} targets={string.Join(",", targets)}");

        Player casterPlayer = GetPlayerById(caster);
        CardDefinition cardDef = GetCardById(cardId);
        SpellDefinition spellDef = cardDef.Spell;

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
        OnCardApplied?.Invoke(caster, cardDef);
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

    CardDefinition GetCardById(int id) => CardRegister.Instance.GetById(id);

    int GetCardId(CardDefinition cardDefinition) => CardRegister.Instance.GetId(cardDefinition);

    #endregion
}
