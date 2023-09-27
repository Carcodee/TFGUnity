using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour
{
    [Header("Player Stats")]
    public NetworkVariable<float> networkSpeed = new NetworkVariable<float>();
    public float speedHolder;
    public NetworkVariable<float> netSprintFactor = new NetworkVariable<float>();


    [Header("Player Components")]
    public GameObject cameraPrefab;
    private CinemachineVirtualCamera cinemachineVirtualCameraInstance;
    bool jump = false;
    bool isSprinting = false;
    bool isCrouching = false;

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
    
    public void CrouchAndSprint()
    {
        if (isSprinting&& Input.GetKey(KeyCode.LeftControl))
        {
            isSprinting = true;
            isCrouching = true;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            SetSprintFactorClientRpc(1.5f);

            isSprinting = true;
        }
        SetSprintFactorClientRpc(1f);
        isCrouching = false;
        isSprinting = false;

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

    [ClientRpc]
    void SetSprintFactorClientRpc(float sprintFactor)
    {
        netSprintFactor.Value = sprintFactor;
    }

    #endregion

}
