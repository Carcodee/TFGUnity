using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerComponentsHandler : NetworkBehaviour
{
    [Header("Player Components")]
    public Canvas canvasPrefab;
    public GameObject cameraPrefab;


    [Header("Ref")]
    private CinemachineVirtualCamera cinemachineVirtualCameraInstance;

    void Start()
    {
        if (IsOwner)
        {
            InstanciateComponents();
        }
    }


    void Update()
    {
        
    }


    void InstanciateComponents()
    {
        GameObject camera = Instantiate(cameraPrefab);
        cinemachineVirtualCameraInstance = camera.GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCameraInstance.LookAt = transform;
        cinemachineVirtualCameraInstance.Follow = transform;
        Canvas canvas = Instantiate(canvasPrefab,transform);
        canvas.GetComponentInChildren<Button>().onClick.AddListener(transform.GetComponent<PlayerStatsController>().OnSpawnPlayer);

    }
}
