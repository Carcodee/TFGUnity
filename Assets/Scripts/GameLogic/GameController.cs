using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    public NetworkVariable<int> numberOfPlayers=new NetworkVariable<int>();
    public NetworkVariable<int> numberOfPlayersAlive=new NetworkVariable<int>();
    public MapLogic mapLogic;


    [Header("Map Logic")]
    public NetworkVariable<float> zoneRadius;
    public NetworkVariable<float> zoneRadiusExpandSpeed;
    public NetworkVariable<float> totalTime;
    public NetworkVariable <float> enemiesSpawnRate;
    public zoneColors[] zoneColors;

    void Start()
    {
        SetNumberOfPlayersAliveServerRpc(2);
        SetNumberOfPlayersServerRpc(2);
        mapLogic=new MapLogic(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0, 0, 0, 0);
        //SetMapLogicServerRpc(numberOfPlayers.Value, numberOfPlayersAlive.Value, 0, 0, 0, 0);
    }

    void Update()
    {
        
    }




    public void ExpandZone()
    {
        zoneRadius.Value = zoneRadiusExpandSpeed.Value*Time.deltaTime;
    }
    public void SetPlayerZones(Transform[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            zoneColors[i] = (zoneColors)i;
            players[i].GetComponent<PlayerStatsController>().SetZoneAsignedStateServerRpc((int)zoneColors[i]);
        }
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
            SetNumberOfPlayersAliveClientServerRpc(numberOfPlayers);
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
            SetNumberOfPlayersClientServerRpc(numberOfPlayersAlive);
        }

    }


    #endregion

    #region ClientRpc
    [ServerRpc]
    public void SetNumberOfPlayersClientServerRpc(int numberOfPlayers)
    {
        this.numberOfPlayers.Value = numberOfPlayers;
    }
    [ServerRpc]
    public void SetNumberOfPlayersAliveClientServerRpc(int numberOfPlayersAlive)
    {
        this.numberOfPlayersAlive.Value = numberOfPlayersAlive;
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
