using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using Unity.Netcode;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    public Rigidbody rb;
    public Vector3 Direction;
    public float speed = 10f;
    public int damage;
    public MeshRenderer meshRenderer;

    public float colorLerpTimer;
    public bool collided = false;
    public BulletHitType bulletHitType;
    void Start()
    {
    collided = false;
    rb.isKinematic = false;
        if (IsOwner)
        {
            StartCoroutine(DestroyBullet());
        }
    }

    void Update()
    {


        if (collided)
        {
            ColorChange(bulletHitType);
        }

    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(2f);
        if (IsServer)
        {
            Destroy(gameObject);
        }
        else
        {
            DestroyServerRpc();
        }
    }
    [ServerRpc]
    public void DestroyServerRpc()
    {
        NetworkManager.Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        rb.velocity = -Direction * speed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent<PlayerStatsController>(out PlayerStatsController enemyRef))
        {
            enemyRef.TakeDamage(damage);
            Debug.Log("Hit: "+ damage);
            collided = true;
            bulletHitType = BulletHitType.Enemy;
        }
        else
        {
            collided = true;
            bulletHitType = BulletHitType.Enviroment;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Hit");
    }



    private void ColorChange(BulletHitType bulletHitType)
    {
        colorLerpTimer += Time.deltaTime;
        Color lerpedCol;
        switch (bulletHitType)
        {
            case BulletHitType.Enemy:

                lerpedCol = Color.Lerp(Color.red, Color.cyan, colorLerpTimer);
                meshRenderer.material.SetColor("_EmissiveColor", lerpedCol);

                break;
            case BulletHitType.Enviroment:
                lerpedCol= Color.Lerp(Color.blue, Color.white, colorLerpTimer);
                meshRenderer.material.SetColor("_EmissiveColor", lerpedCol);
                break;
        }

    }
    public enum BulletHitType
    {
        Enemy,
        Enviroment
    }
}
