using System.Linq;
using Unity.Netcode;
using UnityEngine;

//Es necesario tener una estructura que agrupe el data del hechizo con su caster y targets, para poder aplicar los efectos correctamente. Tambien se podria usar para manejar duracion de hechizos, etc
[System.Serializable]
public struct SpellCastRequest
{
    public SpellDefinition SpellDef { get; private set; }
    public Player Caster { get; private set; }

    public SpellCastRequest(SpellDefinition spellDef, Player caster)
    {
        SpellDef = spellDef;
        Caster = caster;
    }
}

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

    public bool TryCastSpellClientSide(SpellDefinition spellDef)
    {
        return TryCastSpellClientSide(player, new SpellCastRequest(spellDef, player));
    }

    /// <summary>
    /// Client side spell casting. Checks if the player can cast the spell and sends a request to the server to cast it. The server will then validate the spell and apply its effects if valid.
    /// Returned value returns only vlient validation, not server validation, so it can be used to trigger client side casting animations and such.
    /// </summary>
    public bool TryCastSpellClientSide(Player caster, SpellCastRequest castRequest)
    {
        if (!caster.IsOwner) return false;

        if (!CanCastSpell(castRequest)) return false; //Tb se podria animacion de error por mana

        //Animacion de casteo previa a confirmacion de servidor

        var spellID = SpellRegister.Instance.GetId(castRequest.SpellDef);
        CastSpellRpc(spellID, castRequest.Caster.PlayerID);

        return true;
    }

    /// <summary>
    /// Server side spell casting. Validates the spell and applies its effects if valid. This is the only method that should actually apply the spell effects, to avoid desyncs and cheating.
    /// The server will then send an RPC to all clients to trigger the spell effects and animations.
    /// Note that this method is called by the client through an RPC, so it should not be called directly by other server side code. If you want to cast a spell from server side code, you should call ApplySpellRpc directly with a properly constructed SpellCastQuery.
    /// Also note that the validation in this method is minimal, it only checks if the caster has enough mana. More complex validation (like checking if the targets are valid) should be done in ApplySpellRpc, which is called after this method and is responsible for applying the spell effects.
    /// </summary>
    [Rpc(SendTo.Server)]
    public void CastSpellRpc(int spellId, int casterId)
    {
        // if (!IsOwner) return; // Solo el dueño del objeto puede lanzar hechizos
        var castRequest = new SpellCastRequest(SpellRegister.Instance.GetById(spellId), matchManager.GetPlayerById(casterId));
        //Si usase los client ids de ngo
        //var caster = NetworkManager.Singleton.ConnectedClients[(ulong)casterId].PlayerObject.GetComponent<Player>();
        if (castRequest.Caster == null)
        {
            Debug.LogError($"Player with id {casterId} not found", gameObject);
            return;
        }

        if (!CanCastSpell(castRequest))
        {
            Debug.Log($"Player {casterId} tried to cast spell {spellId} but didn't have enough mana", gameObject);
            return;
        }

        if (!castRequest.Caster.ManaManager.ConsumeMana(castRequest.SpellDef.ManaCost))
        {
            Debug.Log($"Player {casterId} failed to consume mana for spell {spellId}", gameObject);
            return;
        }

        ApplySpell(castRequest);
        ApplySpellRpc(spellId, casterId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ApplySpellRpc(int spellId, int casterId)
    {
        var castRequest = new SpellCastRequest(SpellRegister.Instance.GetById(spellId), matchManager.GetPlayerById(casterId));
        ApplySpell(castRequest);
    }

    bool CanCastSpell(SpellCastRequest castRequest)
    {
        return castRequest.Caster.CurrentMana.Value >= castRequest.SpellDef.ManaCost;
    }

    //Esto se ejecuta tanto en cliente como en servidor. Dara problemas? Lo descubriremos.
    //En principio, como no hay criterios sólidos de qué se ejecuta dónde,
    //Voy a dejar que sean los propios efectos de los hechizos los que decidan qué hacer en cada lado,
    //y este método simplemente se encargará de llamar a los efectos del hechizo.
    // De esta forma, cada efecto puede decidir si se ejecuta solo en cliente, solo en servidor, o en ambos.
    void ApplySpell(SpellCastRequest castRequest)
    {
        Player caster = castRequest.Caster;
        var enemyID = matchManager.GetPlayerId(caster) == 0 ? 1 : 0;
        Player enemy = matchManager.GetPlayerById(enemyID);
        ApplyEffectsToTarget(caster, castRequest.SpellDef.OnSelfEffects);
        ApplyEffectsToTarget(enemy, castRequest.SpellDef.OnEnemyEffects);
    }

    void ApplyEffectsToTarget(Player target, StatusEffectDefinition[] effects)
    {
        if (target.TryGetComponent<StatusEffectController>(out var statusEffectController))
        {
            foreach (var effectDef in effects)
            {
                statusEffectController.AddEffect(effectDef);
            }
        }
    }
}
