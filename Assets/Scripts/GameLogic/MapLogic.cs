using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class MapLogic
{
    public int numberOfPlayers;
    public int numberOfPlayersAlive;
    public float zoneRadius;
    public float zoneRadiusExpandSpeed;
    public float totalTime;
    public float enemiesSpawnRate;
    public zoneColors [] zoneColors;
      
    public MapLogic(int numberOfPlayers, int numberOfPlayersAlive, float zoneRadiusExpandSpeed, float totalTime, float enemiesSpawnRate, float zoneRadius)
    {
        this.numberOfPlayers = numberOfPlayers;
        this.numberOfPlayersAlive = numberOfPlayersAlive;
        this.zoneRadiusExpandSpeed = zoneRadiusExpandSpeed;
        this.totalTime = totalTime;
        this.enemiesSpawnRate = enemiesSpawnRate;
        this.zoneRadius = zoneRadius;
        zoneColors = new zoneColors[numberOfPlayers];
    }


    public void ExpandZone()
    {
        zoneRadius+= zoneRadiusExpandSpeed * Time.deltaTime;
    }
    public void SetPlayerZones(Transform[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            zoneColors[i] = (zoneColors)i;
            players[i].GetComponent<PlayerStatsController>().SetZoneAsignedStateServerRpc((int)zoneColors[i]);
        }
    }

}

