using UnityEngine;
using Unity.Cinemachine;

public class PlayerPositioner : MonoBehaviour
{
    [SerializeField] private Transform[] playerTransforms;
    
    public void PositionPlayer(Player p, int playerIndex, bool isOwner = false)
    {
        p.transform.SetPositionAndRotation(playerTransforms[playerIndex].position, 
            playerTransforms[playerIndex].rotation);
        if (isOwner) FindFirstObjectByType<CinemachineCamera>().Follow = p.transform;
    }
}
