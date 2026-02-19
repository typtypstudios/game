using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;

public class NetworkBootstrap : MonoBehaviour
{
    [ContextMenu(nameof(StartHost))]
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    [ContextMenu(nameof(StartClient))]
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    [ContextMenu(nameof(StartServer))]
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    ////Join
    //public void CreateLobby()
    //{
    //    try
    //    {
    //        string joinCode = joinInputText.text;
    //        if (Application.internetReachability == NetworkReachability.NotReachable)
    //            throw new System.Exception("No Internet connection.");
    //        await UnityServices.InitializeAsync();
    //        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    //        if (joinCode.Length > 6) joinCode = joinCode.Substring(0, 6);
    //        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
    //        RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "wss");
    //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
    //        NetworkManager.Singleton.StartClient();
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogError("Invalid code: " + e);
    //        AuthenticationService.Instance.SignOut();
    //    }
    //}
}
