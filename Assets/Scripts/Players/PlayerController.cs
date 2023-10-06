using Cinemachine;
using Newtonsoft.Json.Bson;
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



    [Header("TargetConfigs")]
    public float mouseSensitivity = 100f;
    public float offset = 20.0f;
    public Transform targetPos;
    public Transform headAim;

    [Header("Player Movement")]
    public float rotationFactor;
    bool jump = false;
    bool isSprinting = false;
    bool isCrouching = false;
    float xRotation = 0f;
    float yRotation = 0f;

    void Start()
    {  

        if (IsOwner)
        {
            SetSpeedStateServerRpc(1200);

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
            CreateAimTargetPos();
            RotatePlayerWithMousePos();
            CrouchAndSprint();
            Move();
        }
    }
    void Move()
    {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 move = new Vector3(horizontal, 0, vertical);
            yRotation+= horizontal* rotationFactor * Time.deltaTime;
            transform.rotation = Quaternion.Euler(new Vector3(0, yRotation, 0));
            transform.Translate(move * networkSpeed.Value * netSprintFactor.Value * Time.fixedDeltaTime / 500f);
    }
    void RotatePlayerWithMousePos()
    {
        //float angle = Mathf.Atan2(Input.mousePosition.y, Input.mousePosition.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(new Vector3(0, -angle, 0));

    }
    void CreateAimTargetPos()
    {
        Camera camera = GetComponentInChildren<Camera>();
        
        if (Physics.Raycast(camera.transform.position,camera.transform.forward, out RaycastHit hit,100))
        {
            targetPos.position = hit.point;
            headAim.position=hit.point;
        }

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
