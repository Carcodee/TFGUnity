using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
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
    public TMP_InputField joinCode;
    private string joinText;
    private UnityTransport _transport;
    public GameObject menu;
    public GameObject canvas;
    public string lobbyId;
    
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
            menu.SetActive(false);
            canvas.SetActive(false);
            var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Additive);
            if (status != SceneEventProgressStatus.Started)
            {   
                Debug.LogWarning($"Failed to load {m_SceneName} " +
                                  $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
    }
    public IEnumerator LoadAsynchronously(string sceneName){ // scene name is just the name of the current scene being loaded
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        operation.allowSceneActivation = true;
        while (!operation.isDone){
            if(operation.progress >= 0.9f){
                // var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Additive);
                // if (status != SceneEventProgressStatus.Started)
                // {
                //     Debug.LogWarning($"Failed to load {m_SceneName} " +
                //                       $"with a {nameof(SceneEventProgressStatus)}: {status}");
                // }
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }


    public async void StartHost()
    {
        Allocation a = await RelayService.Instance.CreateAllocationAsync(2,"europe-west2");
        hostCode= await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
        _transport.SetHostRelayData(a.RelayServer.IpV4,(ushort)a.RelayServer.Port,a.AllocationIdBytes, a.Key, a.ConnectionData);


        try
        {
            var createLobbyOptions = new CreateLobbyOptions();
            createLobbyOptions.IsPrivate = false;
            createLobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: hostCode
                    )
                }
            };
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync("New Lobby", 8, createLobbyOptions);
            lobbyId= lobby.Id;
            StartCoroutine(Heartbeat(15));


        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        NetworkManager.Singleton.StartHost();

    }
    
    public IEnumerator Heartbeat(float waitTime)
    {
        var delay= new WaitForSeconds(waitTime);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
    public async void StartClient()
    {
        joinText = joinCode.text;
        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinText);

        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData,a.HostConnectionData);
        NetworkManager.Singleton.StartClient();
    }
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }


}
