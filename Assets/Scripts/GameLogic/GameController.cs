using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    public NetworkVariable<int> numberOfPlayers=new NetworkVariable<int>();
    public NetworkVariable<int> numberOfPlayersAlive=new NetworkVariable<int>();
    public NetworkVariable<MapLogic> mapLogic = new NetworkVariable<MapLogic>();

    zoneColors[] zoneColors;
    public string numberOfPlayersAliveMap;

    private void Awake()
    {

        
        
    }
    
    public override void OnNetworkSpawn()
    {
  


    }

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback+= (clientId) =>
        {
            if (IsServer)
            {
                numberOfPlayers.Value ++;
                numberOfPlayersAlive.Value ++;
                mapLogic.Value.SetMap(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0.5f, 0, 0, 5);
            } 
            else if (IsClient&&IsOwner)
            {
                OnPlayerEnterServerRpc();
                SetMapLogicClientServerRpc(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0.5f, 0, 0, 5);

            }

        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
        {
            if (IsServer)
            {

                mapLogic.Value.SetMap(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0.5f, 0, 0, 5);

            }
            else if (IsClient && IsOwner)
            {
                OnPlayerOutServerRpc();
                SetMapLogicClientServerRpc(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0.5f, 0, 0, 5);

            }

        };

        


    }

    void Update()
    {

    }

    public void CreatePlayerZones()
    {
        zoneColors = new zoneColors[numberOfPlayers.Value];
        for (int i = 0; i < zoneColors.Length; i++)
        {
            zoneColors[i] = (zoneColors)i;
        }
    }
    #region ServerRpc



    #endregion

    #region ClientRpc



    #endregion

    [ServerRpc]
    public void SetMapLogicClientServerRpc(int numberOfPlayers,int numberOfPlayersAlive,float zoneRadiusExpandSpeed,int totalTime,float enemiesSpawnRate,float zoneRadius)
    {
        Debug.Log("Called on client");
        mapLogic.Value.SetMap(numberOfPlayers, numberOfPlayersAlive, zoneRadiusExpandSpeed, totalTime, enemiesSpawnRate, zoneRadius);
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

