using System;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public abstract class SyncEvent
{
    [field: SerializeField] public int Tick { get; set; }

    public SyncEvent(int tick)
    {
        Tick = tick;
    }

    // public abstract Action OnExecute();
    // public abstract void Cancel();
}

[System.Serializable]
public class SpellSyncEvent : SyncEvent
{
    [field: SerializeField] public int SpellId { get; private set; }
    [field: SerializeField] public int CasterId { get; private set; }

    [field: SerializeField] public int[] TargetsId { get; private set; }

    public SpellSyncEvent(int tick, int spellId, int casterId, params int[] targetsId) : base(tick)
    {
        SpellId = spellId;
        CasterId = casterId;
        TargetsId = targetsId;
    }

    // public override void Execute()
    // {
    //     var spell = SpellRegister.Instance.GetSpellById(SpellId);
    //     Debug.Log($"Executing spell {spell.SpellName} casted by player {CasterId} on targets {string.Join(", ", TargetsId)}");
    //     // Aquí iría la lógica para aplicar el hechizo a los objetivos
    // }
}
