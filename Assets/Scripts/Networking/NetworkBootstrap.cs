using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class NetworkBootstrap : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI codeText;

    [ContextMenu(nameof(StartHost))]
    public async void StartHost()
    {
        try
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                throw new System.Exception("No Internet connection.");
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "wss");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            Debug.Log(joinCode);
        }
        catch (System.Exception e) //Si no se puede se hace sign out y se imprime un mensaje de error
        {
            Debug.LogError("Session couldn't be created: " + e);
            AuthenticationService.Instance.SignOut();
        }
    }

    [ContextMenu(nameof(StartClient))]
    public async void StartClient()
    {
        try
        {
            string joinCode = codeText.text;
            if (Application.internetReachability == NetworkReachability.NotReachable)
                throw new System.Exception("No Internet connection.");
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            if (joinCode.Length > 6) joinCode = joinCode.Substring(0, 6);
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "wss");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Invalid code: " + e);
            AuthenticationService.Instance.SignOut();
        }
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
