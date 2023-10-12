using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerZoneController : NetworkBehaviour
{
    [Header("Zone variables")]
    public NetworkVariable<zoneColors> zoneAsigned = new NetworkVariable<zoneColors>();
    public float enemiesSpawnRate;
    public Transform [] spawnPoints;
    public Transform playerAssigned;
    public Transform playerSpawn;
    public Transform enemyContainer;


    [Header("Ref")]
    public EnemyController enemyPrefab;

    public bool isBattleRoyale;

    [Header("Privates")]
    [SerializeField]private float internalSpawnTimer;
    int enemiesSpawned;
    public List<EnemyController> enemies;

    void Start()
    {
        
    }

    void Update()
    {
        if (!IsOwner) return;
        if(!isBattleRoyale)
        {
            internalSpawnTimer += Time.deltaTime;
            if (internalSpawnTimer >= enemiesSpawnRate) 
            {
                SpawnEnemies();
                enemiesSpawned++;
                internalSpawnTimer = 0;
                return;
            }

        }
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
       
        if (IsServer)
        {
            EnemyController enemy = Instantiate(enemyPrefab, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position, Quaternion.identity, enemyContainer);
            enemy.target = playerAssigned;
            enemies.Add(enemy);

            enemies[enemiesSpawned].GetComponent<NetworkObject>().Spawn();
        }
        else
        {
            //Target is only set it on the server, To fix this we need to set the target on the client and then send it to the server
            SpawnEnemyServerRpc(enemiesSpawned);
        }

    }

    [ServerRpc]
    public void SetZoneServerRpc(int val)
    {
        zoneColors zone = (zoneColors)val;
    }
    [ServerRpc]
    public void SpawnEnemyServerRpc(int index)
    {
        EnemyController enemy = Instantiate(enemyPrefab, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position, Quaternion.identity, enemyContainer);
        enemy.target = playerAssigned;
        enemies.Add(enemy);
        enemies[index].GetComponent<NetworkObject>().Spawn();

    }

    #region clientRpc

    [ClientRpc]
    public void SetPlayerOnClientRpc(int index)
    {
        enemies[index].target = playerAssigned; 
    }
    #endregion
}

