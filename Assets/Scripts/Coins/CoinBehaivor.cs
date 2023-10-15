using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
                if (playerRef.OwnerClientId==networkPlayerID.Value)
                {
                    //something happens
                    Debug.Log("coin collected by player " + networkPlayerID.Value);
                }

            }
        }
    }
}
