using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Player Stats")]
    [SerializeField]private float speed;

    [Header("Player Components")]
    public GameObject cameraPrefab;
    private CinemachineVirtualCamera cinemachineVirtualCameraInstance;
    bool jump = false;

    //[Header("Anim")]
    void Start()
    {  

        if (IsOwner)
        {
            GameObject camera= Instantiate(cameraPrefab); 
            speed= 5;
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
            transform.Translate(move * speed * Time.deltaTime);

    }

}
