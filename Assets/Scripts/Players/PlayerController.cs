using Cinemachine;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Networking.Transport.Error;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour
{
    [Header("Player Stats")]
    public PlayerStatsController playerStats;
    public float speedHolder;
    public float speedFactor;


    [Header("Player Components")]
    public GameObject cameraPrefab;
    public BulletController bulletPrefab;
    public Transform cinemachineCameraTarget;

    [SerializeField] private Camera cameraRef;
    [SerializeField] private StateMachineController stateMachineController;

    [Header("TargetConfigs")]
    public float mouseSensitivity = 100f;
    public float offset = 20.0f;
    public Transform targetPos;
    public Transform headAim;
    public Transform spawnBulletPoint;

    [Header("Shoot")]
    public float shootRate = 0.1f;
    public float shootTimer = 0f;


    [Header("Player Movement")]
    public Vector3 move;

    public float rotationFactor;
    public float RotationSmoothTime = 0.1f;
    public float _rotationVelocity;
    public float slidingTime = 0.5f;
    public float slidingSpeed = 3f;
    public float sprintFactor = 2.5f;
    public float crouchFactor = 0.5f;
    public float AimingSpeedFactor = 0.5f;

    private float slidingTimer = 0f;

    [Header("Camera Direction")]
    private int distanceFactor = 100;
    Vector3 cameraDirection;
    Vector3 groundPivot;
    public LayerMask ground;

    [Header("Player Actions")]
    bool jump = false;
    bool isSprinting = false;
    bool isCrouching = false;
    bool isSliding = false;

    float xRotation = 0f;
    float yRotation = 0f;

    public float reloadTime = 1f;
    public float reloadCurrentTime = 0f;
    public bool isReloading = false;

    [Header("Jumping")]
    [SerializeField] private float JumpForce = 5f;
    [SerializeField] public bool isGrounded;
    [SerializeField] public Vector3 groundPos;
    [SerializeField] private float gravityForce = -9.8f;
    public Vector3 _bodyVelocity;


    void Start()
    {

        if (IsOwner)
        {
            SetSpeedStateServerRpc(5);
            stateMachineController.Initializate();
            playerStats = GetComponent<PlayerStatsController>();
        }

    }

    void Update()
    {
        GetComponentInChildren<Camera>().enabled = IsOwner;
        if (IsOwner)
        {
 
            stateMachineController.StateUpdate();
        }
}
    private void FixedUpdate()
    {
        if (IsOwner)
        {
            stateMachineController.StatePhysicsUpdate();
        }

    }
    private void LateUpdate()
    {
        if (IsOwner)
        {
            stateMachineController.StateLateUpdate();
        }

    }
    public void ApplyMovement(Vector3 movement)
    {
        Vector3 motion= movement * playerStats.GetSpeed() * sprintFactor * Time.deltaTime;
        transform.Translate(motion);
        


    }


    public void Move(float x, float y)
    {
        move = new Vector3(x, 0, y);
    }


    public void Jump()
    {
        _bodyVelocity = new Vector3(0, JumpForce, 0) * Time.fixedDeltaTime;
        isGrounded = false;
    }


    public void ApplyGravity()
    {
        Vector3 position = transform.position;
        groundPos = GetGroundPosFromPoint(position);
        if (position.y > groundPos.y)
        {
            _bodyVelocity.y += gravityForce * Mathf.Pow(Time.fixedDeltaTime, 2);
        }
        else if (!isGrounded && _bodyVelocity.y <= 0 && position.y <= groundPos.y)
        {
            //en el suelo
            transform.position = groundPos;
            _bodyVelocity = Vector2.zero;
            isGrounded = true;
            stateMachineController.SetState("Movement");

        }

    }

    public void ApplyJump()
    {
        ApplyGravity();
        transform.position += _bodyVelocity;
    }
    public void RotatePlayer()
    {
        Vector3 playerMovement = new Vector3(move.x, 0, move.z).normalized;

        if (playerMovement.z < 0)
        {
            return;
        }
        float targetAngle = (Mathf.Atan2(0, playerMovement.z) * Mathf.Rad2Deg) + cinemachineCameraTarget.rotation.eulerAngles.y;
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationVelocity, RotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, rotation, 0f);
    }

    public Vector3 GetGroundPosFromPoint(Vector3 pos)
    {
        
        Ray ray = new Ray(pos, -transform.up);
        Debug.DrawRay(pos, -transform.up * 100, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, ground))
        {
            Debug.Log("Ground");

            return hit.point;
        }
        else
        {
            Debug.Log("no ground");

            return transform.position;
        }
    }

    public void Reloading()
    {
        if (playerStats.totalAmmo <= 0)
        {
            playerStats.totalAmmo = 0;
            Debug.Log("Out of ammo find coins to fill your bullets");
            return;
        }
        if (isReloading && reloadCurrentTime < reloadTime)
        {
            reloadCurrentTime += Time.deltaTime;
            if (reloadCurrentTime > reloadTime)
            {
                reloadCurrentTime = 0;
                if (playerStats.totalAmmo <= playerStats.totalBullets)
                {
                    int tempBulletsToFill = playerStats.totalBullets - playerStats.currentBullets;
                    playerStats.currentBullets += playerStats.totalAmmo;
                    playerStats.totalAmmo -= tempBulletsToFill;
                    isReloading = false;
                }
                else
                {
                    playerStats.totalAmmo -= playerStats.totalBullets - playerStats.currentBullets;
                    playerStats.currentBullets += playerStats.totalBullets - playerStats.currentBullets;

                    isReloading = false;

                }
                playerStats.currentBullets = Mathf.Clamp(playerStats.currentBullets, 0, playerStats.totalBullets);

            }
            return;
        }
        if ((Input.GetKeyDown(KeyCode.R) || playerStats.currentBullets <= 0) && (playerStats.currentBullets != playerStats.totalBullets))
        {
            isReloading = true;
            Debug.Log("Reloading");
        }
    }
    public void Shoot()
    {
        shootTimer += Time.deltaTime;
        if (Input.GetKey(KeyCode.Mouse0) && shootTimer > shootRate && playerStats.currentBullets > 0 && !isReloading)
        {
            playerStats.currentBullets--;
            if (IsServer)
            {
                Vector3 direction;
                if (Physics.Raycast(cameraRef.transform.position, cameraRef.transform.forward, out RaycastHit hit, distanceFactor))
                {
                    direction = spawnBulletPoint.position - hit.point;


                }
                else
                {
                    direction = spawnBulletPoint.position - cameraRef.transform.forward * distanceFactor;
                }

                BulletController bullet = Instantiate(bulletPrefab, spawnBulletPoint.position, cinemachineCameraTarget.rotation);
                bullet.Direction = direction.normalized;
                bullet.damage = GetComponent<PlayerStatsController>().GetDamageDone();
                bullet.GetComponent<NetworkObject>().Spawn();


            }
            else
            {
                Vector3 direction;

                if (Physics.Raycast(cameraRef.transform.position, cameraRef.transform.forward, out RaycastHit hit, distanceFactor))
                {
                    direction = spawnBulletPoint.position - hit.point;

                }
                else
                {
                    direction = spawnBulletPoint.position - cameraRef.transform.forward * distanceFactor;
                }

                ShootServerRpc(direction, GetComponent<PlayerStatsController>().GetDamageDone());
            }
            shootTimer = 0;
        }
    }



    public void CreateAimTargetPos()
    {

        if (Physics.Raycast(cameraRef.transform.position, cameraRef.transform.forward, out RaycastHit hit, distanceFactor))
        {
            targetPos.position = hit.point;
            headAim.position = hit.point;
        }
        else
        {

            cameraDirection = cameraRef.transform.forward * distanceFactor;
            targetPos.position = cameraDirection;
            headAim.position = cameraDirection;

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
        speedFactor = val;
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
        }
        else
        {
            SetSpeedClientServerRpc(speed);
        }
    }



    [ServerRpc]
    void SetSpeedClientServerRpc(float speed)
    {
    }

    [ServerRpc]
    void SetSprintFactorServerRpc(float sprintFactor)
    {
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
    jumping,
    reloading,
    Aiming,
    falling

}

public class AmmoBehaviour
{
    int totalAmmo;
    int currentBullets;
    int totalBullets;
    public AmmoBehaviour(int totalAmmo, int currentBullets, int totalBullets)
    {
        this.totalBullets = totalBullets;
        this.totalAmmo = totalAmmo;
        this.currentBullets = currentBullets;
    }
    public void AddAmmo(int ammo)
    {
        totalAmmo += ammo;
    }
    public void Reload()
    {
        if (totalAmmo < totalBullets)
        {
            currentBullets += totalAmmo;
            currentBullets = Mathf.Clamp(currentBullets, 0, totalBullets);
        }
        else
        {
            currentBullets += totalBullets - currentBullets;

        }
        totalAmmo = totalBullets - currentBullets;

    }
}