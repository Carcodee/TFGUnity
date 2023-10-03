using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneManager : NetworkBehaviour
{
    /// INFO: You can remove the #if UNITY_EDITOR code segment and make SceneName public,
    /// but this code assures if the scene name changes you won't have to remember to
    /// manually update it. <summary>
    /// INFO: You can remove the #if UNITY_EDITOR code segment and make SceneName public,
    /// </summary>

    public string hostCode;
    public string joinText;
    private UnityTransport _transport;

    private async void Awake()
    {
     
        _transport= FindObjectOfType<UnityTransport>();

        await Authenticate();

        
    }
#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
    private void OnValidate()
    {
        if (SceneAsset != null)
        {
            m_SceneName = SceneAsset.name;
        }
    }
#endif
    [SerializeField]
    private string m_SceneName;

    public override void OnNetworkSpawn()
    {
        if (IsServer && !string.IsNullOrEmpty(m_SceneName))
        {
            var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Additive);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load {m_SceneName} " +
                      $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
    }


    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }


    public async void StartHost()
    {
        Allocation a = await RelayService.Instance.CreateAllocationAsync(8);
        hostCode= await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
       
        _transport.SetHostRelayData(a.RelayServer.IpV4,(ushort)a.RelayServer.Port,a.AllocationIdBytes, a.Key, a.ConnectionData);

        NetworkManager.Singleton.StartHost();
    }
    public async void StartClient()
    {
        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinText);

        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData,a.HostConnectionData);
        NetworkManager.Singleton.StartClient();
    }
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
}
