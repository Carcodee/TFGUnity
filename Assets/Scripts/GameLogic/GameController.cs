using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    public static GameController instance;
    public NetworkVariable<int> numberOfPlayers=new NetworkVariable<int>();
    public NetworkVariable<int> numberOfPlayersAlive=new NetworkVariable<int>();
    public NetworkVariable<MapLogic> mapLogic = new NetworkVariable<MapLogic>();
    public bool started;
    public NetworkVariable<float> netTimeToStart = new NetworkVariable<float>();
    public float waitingTime;
    public List<Transform> players=new List<Transform>();

        [Header("Zones")]
    public Transform[] spawnPoints;
    public Transform zoneInstances;
    public PlayerZoneController zoneControllerPrefab;
    public List<PlayerZoneController> zoneControllers;
    zoneColors[] zoneColors;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

    }
    
    public override void OnNetworkSpawn()
    {



    }

    void Start()
    {


        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {

            if (IsClient && IsOwner)
            {

                OnPlayerEnterServerRpc();
                SetMapLogicClientServerRpc(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0.5f, 10, 3, 5);
                SetNumberOfPlayerListServerRpc(clientId);
            }

        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
        {

            if (IsClient && IsOwner)
            {

                OnPlayerOutServerRpc();
                SetMapLogicClientServerRpc(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0.5f, 10, 3, 5);
                SetNumberOfPlayerListServerRpc(clientId);

            }

        };



    }

    void Update()
    {
        
            if (IsServer && !started)
            {
                netTimeToStart.Value += Time.deltaTime;

            }
            else if (IsClient && !started&&IsOwner)
            {
                SetTimeToStartServerRpc(Time.deltaTime);
            }
            if (netTimeToStart.Value > waitingTime && !started)
            {
                StartGame();
            }

    }
    public void StartGame()
    {
       
            for (int i = 0; i < numberOfPlayers.Value; i++)
            {
                PlayerZoneController playerZoneController = Instantiate(zoneControllerPrefab, spawnPoints[i].position, Quaternion.identity, zoneInstances);
                playerZoneController.enemiesSpawnRate = mapLogic.Value.enemiesSpawnRate;
                if (IsOwner) playerZoneController.SetZone(i);
                zoneControllers.Add(playerZoneController);
                if (IsServer)
                {
                    zoneControllers[i].playerAssigned = NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.GetComponent<Transform>();
                }
                else if(IsClient&&IsOwner)
                {
                //TODO: fix this
                    GetPlayersServerRpc(i);
                }
        }
            started = true;
        



    }
    public void CreatePlayerZones()
    {
        zoneColors = new zoneColors[numberOfPlayers.Value];
        for (int i = 0; i < zoneColors.Length; i++)
        {
            zoneColors[i] = (zoneColors)i;
        }
    }

    public void CreateZonePrefab(int numberOfPlayers)
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            PlayerZoneController playerZoneController = Instantiate(zoneControllerPrefab, spawnPoints[i].position, Quaternion.identity, zoneInstances);
            playerZoneController.enemiesSpawnRate = mapLogic.Value.enemiesSpawnRate;
            zoneControllers.Add(playerZoneController);
        }

    }


    #region ServerRpc
    [ServerRpc]
    public void SetTimeToStartServerRpc(float time)
    {
        netTimeToStart.Value += time;
    }

    [ServerRpc]
    public void GetPlayersServerRpc(int index)
    {
        zoneControllers[index].playerAssigned = NetworkManager.Singleton.ConnectedClients[(ulong)index].PlayerObject.GetComponent<Transform>();
    }

    [ServerRpc]
    public void SetMapLogicClientServerRpc(int numberOfPlayers,int numberOfPlayersAlive,float zoneRadiusExpandSpeed,int totalTime,float enemiesSpawnRate,float zoneRadius)
    {
        Debug.Log("Called on client");
        mapLogic.Value.SetMap(numberOfPlayers, numberOfPlayersAlive, zoneRadiusExpandSpeed, totalTime, enemiesSpawnRate, zoneRadius);
    }

    [ServerRpc]
    public void SetNumberOfPlayerListServerRpc(ulong clientId) {
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
        {
            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerStatsController>().zoneAsigned.Value = (zoneColors)i;

        }
    }
    [ServerRpc]
    public void SetTimeLeftOnClientsServerRpc(ulong clientId)
    {
        for (ulong i = 0; i < (ulong)NetworkManager.Singleton.ConnectedClients.Count; i++)
        {
            GameObject player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject;
            player.GetComponentInChildren<Canvas>().GetComponentInChildren<TextMeshProUGUI>().text = "Player" + i+1;
        }

    }
    [ServerRpc]
    public void OnPlayerEnterServerRpc()
    {
          numberOfPlayersAlive.Value++;
          numberOfPlayers.Value++;
    }
    [ServerRpc]
    public void OnPlayerOutServerRpc()
    {
        numberOfPlayersAlive.Value--;
        numberOfPlayers.Value--;
    }
    #endregion

}

public enum zoneColors
{
    red,
    blue,
    green,
    yellow,
    purple,
    orange,
    pink,
    brown,
}

