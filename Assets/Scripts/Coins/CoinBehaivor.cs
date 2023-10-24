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


    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (other.TryGetComponent(out PlayerStatsController playerRef))
        {
            if (playerRef.OwnerClientId == networkPlayerID.Value)
            {

                //something happens
                playerRef.LevelUp();

                CoinCollectedClientRpc();

            }

        }
    }

    [ClientRpc]
    public void CoinCollectedClientRpc()
    {
        OnCoinCollected?.Invoke(gameObject.GetComponent<CoinBehaivor>());
    }
}
