using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    public Rigidbody rb;
    public Vector3 Direction;
    public float speed = 10f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        StartCoroutine(DestroyBullet());
    }

    void Update()
    {

            Debug.Log("Bullet Fired");

            rb.velocity = -Direction * speed;
        

    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {

    }
}
