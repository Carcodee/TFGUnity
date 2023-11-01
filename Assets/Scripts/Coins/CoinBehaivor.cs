using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class CoinBehaivor : NetworkBehaviour
{
    public NetworkVariable<ulong> networkPlayerID = new NetworkVariable<ulong>();
    public NetworkVariable <int> zoneAssigned;

    public static Action<CoinBehaivor> OnCoinCollected;
    public GameObject coinEffectPrefab;

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            if (other.TryGetComponent(out PlayerStatsController playerRef))
            {
                if (playerRef.OwnerClientId == networkPlayerID.Value)
                {
                    //something happens
                    playerRef.LevelUp();
                    playerRef.RefillAmmo();

                    CoinCollectedClientRpc();
                }
            }
        }
        else
        {

        }
    }

    [ClientRpc]
    public void CoinCollectedClientRpc()
    {
        Instantiate(coinEffectPrefab, transform.position, Quaternion.identity);
        OnCoinCollected?.Invoke(gameObject.GetComponent<CoinBehaivor>());
    }
}
