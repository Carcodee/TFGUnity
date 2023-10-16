using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class CoinBehaivor : NetworkBehaviour
{

    public NetworkVariable<ulong> networkPlayerID = new NetworkVariable<ulong>();

    zoneColors zone;



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
                    TakeCoinClientRpc(playerRef.OwnerClientId);
                }

            }
        }
    }

    #region client
    [ClientRpc]
    public void TakeCoinClientRpc(ulong playerID)
    {
        Debug.Log("coin collected by player " + playerID);
    }
    #endregion
}
