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
            SetSpeedStateServerRpc(8);
            GameObject camera = Instantiate(cameraPrefab); 
            cinemachineVirtualCameraInstance = camera.GetComponentInChildren<CinemachineVirtualCamera>();
            cinemachineVirtualCameraInstance.LookAt = transform;
            cinemachineVirtualCameraInstance.Follow = transform;
        }
    }

    void Update()
    {
        GetComponentInChildren<Camera>().enabled = IsOwner;


    }
    private void FixedUpdate()
    {
        if (IsOwner)
        {
            CrouchAndSprint();
            Move();
        }
    }
    void Move()
    {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 move = new Vector3(horizontal, 0, vertical);
            transform.Translate(move * networkSpeed.Value * netSprintFactor.Value * NetworkManager.Singleton.LocalTime.TimeAsFloat / 500f);
    }
    
    public void CrouchAndSprint()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            SetSprintFactor(0.5f);
            return;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            SetSprintFactor(1.5f);
            isSprinting = true;
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl))
            {
                isCrouching = true;
                return;
            }
            return;
        }
        

        SetSprintFactor(1f);
        isCrouching = false;
        isSprinting = false;

    }

    public void SetSprintFactor(float val)
    {
        if (IsServer)
        {
            netSprintFactor.Value = val;
        }
        else
        {
            SetSprintFactorServerRpc(val);
        }
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
            SetSpeedClientServerRpc(speed);
        }
    }



    [ServerRpc]
    void SetSpeedClientServerRpc(float speed)
    {
        networkSpeed.Value = speed;
    }

    [ServerRpc]
    void SetSprintFactorServerRpc(float sprintFactor)
    {
        netSprintFactor.Value = sprintFactor;
    }

    #endregion

}
