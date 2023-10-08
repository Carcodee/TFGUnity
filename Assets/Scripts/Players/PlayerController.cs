using Cinemachine;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerController : NetworkBehaviour
{
    [Header("Player Stats")]
    public NetworkVariable<float> networkSpeed = new NetworkVariable<float>();
    public float speedHolder;
    public NetworkVariable<float> netSprintFactor = new NetworkVariable<float>();


    [Header("Player Components")]
    public GameObject cameraPrefab;
    public Transform cinemachineCameraTarget;


    [Header("TargetConfigs")]
    public float mouseSensitivity = 100f;
    public float offset = 20.0f;
    public Transform targetPos;
    public Transform headAim;

    [Header("Player Movement")]
    Vector3 move;

    public float rotationFactor;
    public float RotationSmoothTime = 0.1f;
    public float _rotationVelocity ;
    public float slidingTime = 0.5f;
    public float slidingSpeed = 3f;
    public float sprintFactor = 2.5f;
    public float crouchFactor = 0.5f;

    private float slidingTimer = 0f;

    [Header("Player Actions")]

    bool jump = false;
    bool isSprinting = false;
    bool isCrouching = false;
    bool isSliding = false;

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
            CrouchAndSprint();
            Move();
            RotatePlayer();

        }
    }
    void Move()
    {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            move = new Vector3(horizontal, 0, vertical);
            //yRotation+= horizontal* rotationFactor * Time.deltaTime;
            //transform.rotation = Quaternion.Euler(new Vector3(0, yRotation, 0));
            transform.Translate(move * networkSpeed.Value * netSprintFactor.Value * Time.fixedDeltaTime / 500f);
    }
    void RotatePlayer()
    {
        Vector3 playerMovement=new Vector3(move.x,0,move.z).normalized;

        if (playerMovement.z > 0)
        {
            float targetAngle = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg +cinemachineCameraTarget.rotation.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }

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
        if (isSliding)
        {
            slidingTimer += Time.fixedDeltaTime;
            if (slidingTimer > slidingTime)
            {
                isSliding = false;
                slidingTimer = 0;
            }
            return;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            SetSprintFactor(sprintFactor);
            isSprinting = true;
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.LeftAlt))
            {
                isSliding = true;
                SetSprintFactor(slidingSpeed);
                return;
            }
            return;
        }
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            isCrouching = true;
            SetSprintFactor(crouchFactor);
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
