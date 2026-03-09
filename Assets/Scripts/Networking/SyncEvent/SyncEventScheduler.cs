using Unity.Netcode;
using UnityEngine;

public enum SyncEventType
{
    SpellCast,
    // Otros tipos de eventos de sincronización pueden ir aquí
}

public class SyncEventScheduler : NetworkSingleton<SyncEventScheduler>
{
    //Client verification
    public void TryScheduleEvent(SyncEventType syncEvent, int startTick)
    {

    }

    [Rpc(SendTo.Everyone)]
    public void ScheduleEventRpc(int syncEvent, int startTick)
    {
        // ScheduleEvent(T, startTick);
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.NetworkTickSystem.Tick += Tick;
    }

    private void Tick()
    {
        Debug.Log($"Tick: {NetworkManager.LocalTime.Tick}");
    }

    public override void OnNetworkDespawn() // don't forget to unsubscribe
    {
        NetworkManager.NetworkTickSystem.Tick -= Tick;
    }
}
