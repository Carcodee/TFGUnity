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
    public GameObject cameraZoomPrefab;


    [Header("Ref")]
    private CinemachineVirtualCamera cinemachineVirtualCameraInstance;

    [Header("UI")]
    public TextMeshProUGUI playerNameText;
    float timer = 0;
    void Start()
    {
        if (IsOwner)
        {
            InstanciateComponents();
        }
    }


    void Update()
    {
        if (IsOwner)
        {
            //timer += Time.deltaTime;
            //playerNameText.text ="Time left for Battle royale "+ (GameController.instance.mapLogic.Value.totalTime - timer).ToString("0.0");
            //if (timer> GameController.instance.mapLogic.Value.totalTime)
            //{
            //    Debug.Log("BattleRoyale starts");
            //}
        }
    }


    void InstanciateComponents()
    {
        GameObject camera = Instantiate(cameraPrefab);
        cinemachineVirtualCameraInstance = camera.GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCameraInstance.Follow = transform;
        Canvas canvas = Instantiate(canvasPrefab,transform);
        canvas.GetComponentInChildren<Button>().onClick.AddListener(transform.GetComponent<PlayerStatsController>().OnSpawnPlayer);
        playerNameText = canvas.GetComponentInChildren<TextMeshProUGUI>();
    }
}
