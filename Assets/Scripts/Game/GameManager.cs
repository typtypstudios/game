using Unity.Netcode;
using UnityEngine;

public enum GameState
{
    Waiting,
    Playing,
    Finished
}

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public NetworkVariable<GameState> CurrentState = new();
    public NetworkVariable<int> RitualSeed = new();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            RitualSeed.Value = Random.Range(0, 100000);
            CurrentState.Value = GameState.Playing;
        }
    }
}
