using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    public NetworkVariable<int> numberOfPlayers=new NetworkVariable<int>();
    public NetworkVariable<int> numberOfPlayersAlive=new NetworkVariable<int>();
    public MapLogic mapLogic;


    public override void OnNetworkSpawn()
    {
        SetNumberOfPlayersAliveServerRpc(2);
        SetNumberOfPlayersServerRpc(2);
        SetMapLogicServerRpc(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0.5f, 0, 0, 5);
        //SetMapLogicServerRpc(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0, 0, 0, 0);
    }
    void Start()
    {
        if (IsClient)
        {
            SetMapLogicServerRpc(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0.5f, 0, 0, 5);
        }

    }

    void Update()
    {
        
    }


    #region ServerRpc
    [ServerRpc]
    public void SetNumberOfPlayersServerRpc(int numberOfPlayers)
    {
        if (IsServer)
        {
            this.numberOfPlayers.Value = numberOfPlayers;
        }
        else
        {
            SetNumberOfPlayersClientRpc(numberOfPlayers);
        }

    }
    [ServerRpc]
    public void SetNumberOfPlayersAliveServerRpc(int numberOfPlayersAlive)
    {
        if(IsServer)
        {
            this.numberOfPlayersAlive.Value = numberOfPlayersAlive;
        }
        else
        {
            SetNumberOfPlayersAliveClientRpc(numberOfPlayersAlive);
        }

    }
    [ServerRpc]
    public void SetMapLogicServerRpc(int numberOfPlayers, int numberOfPlayersAlive, float zoneRadiusExpandSpeed, int totalTime, float enemiesSpawnRate, float zoneRadius)
    {
        if (IsServer)
        {
            mapLogic = new MapLogic(numberOfPlayers, numberOfPlayersAlive, zoneRadiusExpandSpeed, totalTime, enemiesSpawnRate, zoneRadius);
        }
        else
        {
            SetMapLogicClientRpc(numberOfPlayers, numberOfPlayersAlive, zoneRadiusExpandSpeed, totalTime, enemiesSpawnRate, zoneRadius);

        }
    }
    #endregion

    #region ClientRpc

    [ClientRpc]
    public void SetNumberOfPlayersClientRpc(int numberOfPlayers)
    {
        this.numberOfPlayers.Value = numberOfPlayers;
    }
    [ClientRpc]
    public void SetNumberOfPlayersAliveClientRpc(int numberOfPlayersAlive)
    {
        this.numberOfPlayersAlive.Value = numberOfPlayersAlive;
    }

    #endregion
    [ClientRpc]
    public void SetMapLogicClientRpc(int numberOfPlayers,int numberOfPlayersAlive,float zoneRadiusExpandSpeed,int totalTime,float enemiesSpawnRate,float zoneRadius)
    {
        mapLogic = new MapLogic(numberOfPlayers, numberOfPlayersAlive, zoneRadiusExpandSpeed, totalTime, enemiesSpawnRate, zoneRadius);
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
