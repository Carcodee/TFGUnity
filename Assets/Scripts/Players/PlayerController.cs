using Cinemachine;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour,IMovable
{
    [Header("Player Stats")]
    public NetworkVariable<float> networkSpeed = new NetworkVariable<float>();
    public float speedHolder;
    public NetworkVariable<float> netSprintFactor = new NetworkVariable<float>();


    [Header("Player Components")]
    public GameObject cameraPrefab;
    public BulletController bulletPrefab;
    public Transform cinemachineCameraTarget;
    [SerializeField]private Camera cameraRef;

    [Header("TargetConfigs")]
    public float mouseSensitivity = 100f;
    public float offset = 20.0f;
    public Transform targetPos;
    public Transform headAim;
    public Transform spawnBulletPoint;

    [Header("Player Movement")]
    Vector3 move;
    public static PlayerMovementStates playerMovementStates;

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

    [Header("Interfaces")]
    private IMovable _iMovable;

    void Start()
    {  

        if (IsOwner)
        {
            _iMovable = GetComponent<IMovable>();
            SetSpeedStateServerRpc(5);
        }

    }

    void Update()
    {
        GetComponentInChildren<Camera>().enabled = IsOwner;
        if (IsOwner)
        {
            CreateAimTargetPos();
            CrouchAndSprint();
            Move();
            RotatePlayer();
            Shoot();
        }
    }
    private void FixedUpdate()
    {


    }
    private void LateUpdate()
    {
        if (IsOwner)
        {

            transform.Translate(move * networkSpeed.Value * netSprintFactor.Value * Time.deltaTime);
        }

    }

    public void OnMovementStateChanged(PlayerMovementStates newMovementState)
    {
        playerMovementStates = newMovementState;
        switch (playerMovementStates)
        {
            case PlayerMovementStates.runnning:
                transform.Translate(move * networkSpeed.Value * netSprintFactor.Value * Time.deltaTime);
                break;
            case PlayerMovementStates.sprinting:
                isSprinting = true;
                SetSprintFactor(sprintFactor);
                transform.Translate(move * networkSpeed.Value * netSprintFactor.Value * Time.deltaTime);

                break;
            case PlayerMovementStates.crouching:
                isCrouching = true;
                SetSprintFactor(crouchFactor);
                transform.Translate(move * networkSpeed.Value * netSprintFactor.Value * Time.deltaTime);

                break;
            case PlayerMovementStates.sliding:
                isSprinting = true;
                isSliding = true;
                slidingTimer += Time.fixedDeltaTime;
                if (slidingTimer > slidingTime)
                {
                    isSliding = false;
                    slidingTimer = 0;
                }
                SetSprintFactor(slidingSpeed);
                transform.Translate(move * networkSpeed.Value * netSprintFactor.Value * Time.deltaTime);
                break;
            case PlayerMovementStates.jumping:
                break;
            default:
                break;
        }
    }


    void Move()
    {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            move = new Vector3(horizontal, 0, vertical);
    }


    void RotatePlayer()
    {
        Vector3 playerMovement=new Vector3(move.x,0,move.z).normalized;
        if (playerMovement.z < 0)
        {
            return;
        }
        if (playerMovement.x!=0)
        {
            transform.LookAt(headAim.transform.position, transform.up);
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            return;
        }

        float targetAngle = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg + cinemachineCameraTarget.rotation.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        

    }
    public void Shoot()
    {

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (IsServer)
            {
                Vector3 direction;
                //todo: fix this when player target a bullet ratates around because it not ignoring the bullet layer
                if (Physics.Raycast(cameraRef.transform.position, cameraRef.transform.forward, out RaycastHit hit, 10000))
                {
                    direction = spawnBulletPoint.position - hit.point;
  

                }
                else
                {
                     direction = spawnBulletPoint.position - cameraRef.transform.forward * 10000;
                }

                BulletController bullet = Instantiate(bulletPrefab, spawnBulletPoint.position, cinemachineCameraTarget.rotation);
                bullet.Direction = direction.normalized;
                bullet.damage = GetComponent<PlayerStatsController>().GetDamageDone();
                bullet.GetComponent<NetworkObject>().Spawn();


            }
            else
            {
                Vector3 direction;

                if (Physics.Raycast(cameraRef.transform.position, cameraRef.transform.forward, out RaycastHit hit, 10000))
                {
                    direction = spawnBulletPoint.position - hit.point;

                }
                else
                {
                    direction = spawnBulletPoint.position - cameraRef.transform.forward * 10000;
                }
                ShootServerRpc(direction, GetComponent<PlayerStatsController>().GetDamageDone());
            }
        }
    }

    

    void CreateAimTargetPos()
    {
        
        if (Physics.Raycast(cameraRef.transform.position, cameraRef.transform.forward, out RaycastHit hit,10000))
        {
            targetPos.position = hit.point;
            headAim.position=hit.point;
        }
        else
        {
            Vector3 direction = cameraRef.transform.forward * 100;
            targetPos.position = direction;
            headAim.position = direction;

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
            isSprinting = false;
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
    public void ShootServerRpc(Vector3 dir, int damage)
    {
             BulletController bullet = Instantiate(bulletPrefab, spawnBulletPoint.position, cinemachineCameraTarget.rotation);
             bullet.Direction = dir.normalized;
             bullet.damage = damage;
             bullet.GetComponent<NetworkObject>().Spawn();
    }

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

public enum PlayerMovementStates
{
    idle,
    runnning,
    sprinting,
    crouching,
    sliding,
    aiming,
    jumping
}