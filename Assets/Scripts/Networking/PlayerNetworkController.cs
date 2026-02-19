using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(RitualManager))]
[RequireComponent(typeof(ManaSystem))]
[RequireComponent(typeof(CorruptionSystem))]
[RequireComponent(typeof(SpellCaster))]
public class PlayerNetworkController : NetworkBehaviour
{
    public NetworkVariable<int> RitualProgress = new(0);
    public NetworkVariable<int> Mana = new(0);
    public NetworkVariable<int> Corruption = new(0);

    private RitualManager ritualManager;
    private ManaSystem manaSystem;
    private CorruptionSystem corruptionSystem;
    private SpellCaster spellCaster;

    private void Awake()
    {
        ritualManager = GetComponent<RitualManager>();
        manaSystem = GetComponent<ManaSystem>();
        corruptionSystem = GetComponent<CorruptionSystem>();
        spellCaster = GetComponent<SpellCaster>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            ritualManager.GenerateRitual(GameManager.Instance.RitualSeed.Value);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        foreach (char c in Input.inputString)
        {
            SubmitCharacterServerRpc(c);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CastSpellServerRpc(0);
        }
    }

    [ServerRpc]
    private void SubmitCharacterServerRpc(char character)
    {
        if (!IsServer) return;

        bool correct = ritualManager.Validate(character);

        if (correct)
        {
            manaSystem.AddMana(1);
            RitualProgress.Value++;
            Mana.Value = manaSystem.CurrentMana;
        }
        else
        {
            corruptionSystem.AddCorruption(1);
            Corruption.Value = corruptionSystem.CurrentCorruption;
        }

        CheckWinCondition();
    }

    [ServerRpc]
    private void CastSpellServerRpc(int spellIndex)
    {
        if (!IsServer) return;

        if (!spellCaster.CanCast(spellIndex)) return;

        spellCaster.Cast(spellIndex);

        Mana.Value = manaSystem.CurrentMana;
        Corruption.Value = corruptionSystem.CurrentCorruption;

        PlaySpellEffectClientRpc(spellIndex);
    }

    [ClientRpc]
    private void PlaySpellEffectClientRpc(int spellIndex)
    {
        // Aquí solo VFX / SFX
        Debug.Log("Spell visual triggered: " + spellIndex);
    }

    private void CheckWinCondition()
    {
        if (ritualManager.IsCompleted())
        {
            GameManager.Instance.CurrentState.Value = GameState.Finished;
        }

        if (corruptionSystem.CurrentCorruption >= 100)
        {
            GameManager.Instance.CurrentState.Value = GameState.Finished;
        }
    }
}
