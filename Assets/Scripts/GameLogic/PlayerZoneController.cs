using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerZoneController : NetworkBehaviour
{

    public NetworkVariable<zoneColors> zoneAsigned = new NetworkVariable<zoneColors>();
    public float enemiesSpawnRate;
    bool isBattleRoyale;


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
