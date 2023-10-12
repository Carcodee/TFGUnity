using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor.Networking.PlayerConnection;
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

        //Check if a player connected to the server
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            if (IsServer) {
                AddPlayerToListClientRpc();
            }

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

    /// <summary>
    /// create the player zones with the player transforms
    /// </summary>
    public void StartGame()
    {
            for (int i = 0; i < numberOfPlayers.Value; i++)
            {
                if (IsOwner)
                {

                    CreateZonesOnNet(i);

                }
                if (IsServer) SetPlayerPosClientRpc(zoneControllers[i].playerSpawn.position, i);
            }
            started = true;
    }

    /// <summary>
    /// Set Each player a zone
    /// </summary>
    public void CreatePlayerZones()
    {
        zoneColors = new zoneColors[numberOfPlayers.Value];
        for (int i = 0; i < zoneColors.Length; i++)
        {
            zoneColors[i] = (zoneColors)i;
        }
    }


    

    public void CreateZonesOnNet(int index)
    {
        if (IsServer)
        {
            PlayerZoneController playerZoneController = Instantiate(zoneControllerPrefab, spawnPoints[index].position, Quaternion.identity, zoneInstances);
            playerZoneController.enemiesSpawnRate = mapLogic.Value.enemiesSpawnRate;
            playerZoneController.SetZone(index);
            zoneControllers.Add(playerZoneController);
            zoneControllers[index].playerAssigned = players[index];
            zoneControllers[index].isBattleRoyale = false;
            zoneControllers[index].GetComponent<NetworkObject>().Spawn();
            SetPlayerOnClientRpc(index);
        }
        else
        {
            SpawnZonesInNetworkServerRpc(index);
        }
    }

    #region ServerRpc
    [ServerRpc]
    public void SetTimeToStartServerRpc(float time)
    {
        netTimeToStart.Value += time;
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


/// <summary>
/// Spawn the zones on the network
/// </summary>
    [ServerRpc]
    public void SpawnZonesInNetworkServerRpc(int index)
    {
        PlayerZoneController playerZoneController = Instantiate(zoneControllerPrefab, spawnPoints[index].position, Quaternion.identity, zoneInstances);
        playerZoneController.enemiesSpawnRate = mapLogic.Value.enemiesSpawnRate;
        playerZoneController.SetZone(index);
        zoneControllers.Add(playerZoneController);
        zoneControllers[index].playerAssigned = players[index];
        zoneControllers[index].isBattleRoyale = false;
        zoneControllers[index].GetComponent<NetworkObject>().Spawn();
        
    }
    #endregion


    #region clientRpc
    [ClientRpc]
    public void SetPlayerPosClientRpc(Vector3 pos, int playerIndex)
    {
        players[playerIndex].position = pos;
    }
    [ClientRpc]
    public void SetPlayerOnClientRpc(int index)
    {
        zoneControllers[index].playerAssigned = players[index];
        Debug.Log("Assigned On Client");

    }

    [ClientRpc]
    public void AddPlayerToListClientRpc()
    {
        players.Clear();

        GameObject[] playersInScene = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject index in playersInScene)
        {
            players.Add(index.transform);
            Debug.Log("Connected Client");
        }
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

