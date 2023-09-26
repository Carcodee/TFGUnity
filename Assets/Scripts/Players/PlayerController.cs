using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Player Stats")]
    public NetworkVariable<float> networkSpeed = new NetworkVariable<float>();
    public float speedHolder;


    [Header("Player Components")]
    public GameObject cameraPrefab;
    private CinemachineVirtualCamera cinemachineVirtualCameraInstance;
    bool jump = false;

    //[Header("Anim")]
    void Start()
    {  

        if (IsOwner)
        {

            SetSpeedStateServerRpc(5);
            GameObject camera = Instantiate(cameraPrefab); 
            cinemachineVirtualCameraInstance = camera.GetComponentInChildren<CinemachineVirtualCamera>();
            cinemachineVirtualCameraInstance.LookAt = transform;
            cinemachineVirtualCameraInstance.Follow = transform;
        }
    }

    void Update()
    {
        GetComponentInChildren<Camera>().enabled = IsOwner;

        if (IsOwner)
        {
            Move();
        }
    }
    
    void Move()
    {

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 move = new Vector3(horizontal, 0, vertical);
            transform.Translate(move * networkSpeed.Value * Time.deltaTime);

    }



    #region ServerRpc
    [ServerRpc]
    public void SetSpeedStateServerRpc(float speed)
    {

        if (IsServer)
        {
            networkSpeed.Value = speed;
        }
        else
        {
            SetSpeedClientClientRpc(speed);
        }
    }

    [ClientRpc]
    void SetSpeedClientClientRpc(float speed)
    {
        networkSpeed.Value = speed;
    }


    #endregion

}
