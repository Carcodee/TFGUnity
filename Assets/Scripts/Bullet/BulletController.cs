using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using Unity.Netcode;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    [Header("Ref")] 
    public Camera mainCam;
    public Rigidbody rb;
    public Vector3 Direction;
    public float speed = 10f;
    public NetworkVariable<int> damage;
    public MeshRenderer meshRenderer;

    public float colorLerpTimer;
    public bool collided = false;
    public BulletHitType bulletHitType;
    
    [Header("Spawns Effects")]
    public GameObject onHitEffectPrefab;
    public FloatingTextController floatingTextPrefab;
    
    
    void Start()
    {
        Physics.IgnoreCollision(GameController.instance.sphereRadius.GetComponent<Collider>(), GetComponent<Collider>());
        collided = false;
        rb.isKinematic = true;
        mainCam = Camera.main;
        Destroy(gameObject,2.0f);
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
        if (!collided)
        {
            transform.position += -Direction * speed * Time.deltaTime;
        }
        else
        {
            rb.velocity = -Direction * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent<PlayerStatsController>(out PlayerStatsController enemyRef))
        {
            enemyRef.TakeDamage(damage.Value);
        }
        transform.GetComponent<Collider>().isTrigger = false;
        rb.isKinematic = false;
        collided = true;
        Debug.Log(other.transform.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent<PlayerStatsController>(out PlayerStatsController enemyRef))
        {
            collided = true;
            bulletHitType = BulletHitType.Enemy;
            Debug.Log(enemyRef.name);
        }
        else
        {
            Debug.Log(collision.transform.name);
            collided = true;
            bulletHitType = BulletHitType.Enviroment;
        }
        // FloatingTextController floatingText= Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        // floatingText.text.text = "Damage: "+damage.Value.ToString();
        // floatingText.mainCam = mainCam;
        Direction= new Vector3 (UnityEngine.Random.RandomRange(0.0f,1.0f), UnityEngine.Random.RandomRange(0.0f, 1.0f), UnityEngine.Random.RandomRange(0.0f, 1.0f));
        SpawnHitEffectClientRpc();

        speed = 1;   
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
    [ClientRpc]
    public void SpawnHitEffectClientRpc()
    {
        Instantiate(onHitEffectPrefab, transform.position, Quaternion.identity);
    }
    public enum BulletHitType
    {
        Enemy,
        Enviroment
    }
}
