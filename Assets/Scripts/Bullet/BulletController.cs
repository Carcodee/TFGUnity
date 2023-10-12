using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    public Rigidbody rb;
    public Vector3 Direction;
    public float speed = 10f;
    public int damage;
    void Start()
    {
        rb.isKinematic = false;
        if (IsOwner)
        {
            StartCoroutine(DestroyBullet());
        }
    }

    void Update()
    {

            Debug.Log("Bullet Fired");

            rb.velocity = -Direction * speed;
        

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
          Destroy(gameObject);
    }
    private void FixedUpdate()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (TryGetComponent<EnemyBase>(out EnemyBase enemyRef))
        {
            enemyRef.TakeDamage(damage);
        }
    }
}
