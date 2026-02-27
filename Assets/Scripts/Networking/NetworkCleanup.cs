using Unity.Netcode;
using UnityEngine;

public class NetworkCleanup : MonoBehaviour
{
    void Awake()
    {
        var nm = NetworkManager.Singleton;

        if (nm != null)
        {
            nm.Shutdown(); // por seguridad
            Destroy(nm.gameObject);
        }
    }
}
