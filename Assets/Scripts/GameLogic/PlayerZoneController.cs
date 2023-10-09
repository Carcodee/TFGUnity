using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerZoneController : NetworkBehaviour
{

    public NetworkVariable<zoneColors> zoneAsigned = new NetworkVariable<zoneColors>();
    public float enemiesSpawnRate;
    public Transform [] spawnPoints;
    public Transform playerAssigned;
    bool isBattleRoyale;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetZone(int val)
    {
        if (IsServer)
        {
            zoneColors zone = (zoneColors)val;
        }
        else
        {
            SetZoneServerRpc(val);
        }
    }
    public void SpawnEnemies()
    {

    }
    [ServerRpc]
    public void SetZoneServerRpc(int val)
    {
        zoneColors zone = (zoneColors)val;
    }
}
