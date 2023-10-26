using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Windows;
using StarterAssets;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerComponentsHandler : NetworkBehaviour
{
    public PlayerInput playerInput;

    public StarterAssetsInputs input;
    public InputActionAsset playerControls;

    [Header("Player Components")]
    public Canvas canvasPrefab;
    public GameObject cameraPrefab;
    public GameObject cameraZoomPrefab;
    private Rigidbody rb;

    [Header("Ref")]
    private CinemachineVirtualCamera cinemachineVirtualCameraInstance;
    [Header("UI")]
    public TextMeshProUGUI playerNameText;
    float timer = 0;

    [Header("Cinemachine Camera config")]
    public Transform cinemachineCameraTarget;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;
    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    private bool IsCurrentDeviceMouse=true;
    private const float _threshold = 0;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            InstanciateComponents();

        }

    }

    void Start()
    {
        if (IsOwner)
        {
    
            // Attach input events
            playerInput.onActionTriggered += HandleAction;
        }

    }
    void Update()
    {
        if (IsOwner)
        {
 
        }
    }
    private void LateUpdate()
    {
        if (IsOwner)
        {
            CameraRotation(input.look);
        }
    }
    private void HandleAction(InputAction.CallbackContext context)
    {
        if (context.action.name == "ExactNameOfYourLookAction") // Replace with your action's name
        {
            CameraRotation(input.look);
        }
        // Handle other actions as needed
    }

    void InstanciateComponents()
    {
        //_playerInput = GetComponent<PlayerInput>();
        //_input = GetComponent<StarterAssetsInputs>();
        input = transform.GetComponent<StarterAssetsInputs>();
        playerInput.enabled = true;
        if (playerInput.actions != playerControls)
        {
            playerInput.actions = playerControls;
        }


        _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
        GameObject camera = Instantiate(cameraPrefab);
        cinemachineVirtualCameraInstance = camera.GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCameraInstance.Follow = cinemachineCameraTarget;
        Canvas canvas = Instantiate(canvasPrefab,transform);
        canvas.GetComponentInChildren<Button>().onClick.AddListener(transform.GetComponent<PlayerStatsController>().OnSpawnPlayer);
        playerNameText = canvas.GetComponentInChildren<TextMeshProUGUI>();


        rb= GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void CameraRotation(Vector3 look)
    {
        // if there is an input and camera position is not fixed
        if (look.sqrMagnitude >= _threshold )
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            
            _cinemachineTargetYaw += look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += look.y * deltaTimeMultiplier;
        }
        // clamp our rotations so our values are limited 360 degrees

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
