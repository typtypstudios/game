using System;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using UnityEngine;

public class LobbyEventsHandler
{
    private ILobbyEvents lobbyEvents;
    private Action<ILobbyChanges> onChanges;
    private Action onLost;

    public async Task<bool> Subscribe(string lobbyId, Action<ILobbyChanges> onChangesCallback, Action onLostCallback)
    {
        onChanges = onChangesCallback;
        onLost = onLostCallback;

        LobbyEventCallbacks lobyCallbacks = new LobbyEventCallbacks();
        lobyCallbacks.LobbyChanged += HandleLobbyChanged;
        lobyCallbacks.KickedFromLobby += HandleKicked;
        lobyCallbacks.LobbyEventConnectionStateChanged += HandleConnectionStateChanged;

        try
        {
            lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyId, lobyCallbacks);
            return true;
        }
        catch (LobbyServiceException ex)
            when (ex.Reason == LobbyExceptionReason.AlreadySubscribedToLobby)
        {
            Debug.LogWarning($"Already subscribed to lobby [{lobbyId}].");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning("SubscribeToLobbyEvents failed: " + e.Message);
            return false;
        }
    }

    public async Task Unsubscribe()
    {
        if (lobbyEvents != null)
        {
            try { await lobbyEvents.UnsubscribeAsync(); }
            catch (Exception e) { Debug.LogWarning("Unsubscribe failed: " + e.Message); }
            lobbyEvents = null;
        }
        onChanges = null;
        onLost = null;
    }

    private void HandleLobbyChanged(ILobbyChanges changes)
    {
        if (changes.LobbyDeleted)
        {
            onLost?.Invoke();
            return;
        }
        onChanges?.Invoke(changes);
    }

    private void HandleKicked() => onLost?.Invoke();

    private void HandleConnectionStateChanged(LobbyEventConnectionState state)
        => Debug.Log($"[LobbyEvents] State: {state}");
}