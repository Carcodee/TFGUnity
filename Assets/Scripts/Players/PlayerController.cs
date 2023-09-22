using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float speed = 5f;
    public GameObject cameraPrefab;
    private CinemachineVirtualCamera cinemachineVirtualCameraInstance;

    void Start()
    {  

        if (IsOwner)
        {
            GameObject camera= Instantiate(cameraPrefab); 
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
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(horizontal, 0, vertical);

        transform.Translate(move * speed * Time.deltaTime);
    }
}
