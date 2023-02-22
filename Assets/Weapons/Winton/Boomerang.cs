using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : ProjectileBase
{
    public Vector3 dir;
    private float rotationSpeed, speed;
    private Rigidbody rb;
    [SerializeField] BoomerangWeapon boomerangWeapon;
    public enum BoomererangState
    {
        NONE,
        THROW,
        RECOIL,
    }
    public BoomererangState boomererangState;

    void Start()
    {
        speed = 1000f;
        damage = boomerangWeapon.damage[0];
        rotationSpeed = 100f;
        boomererangState = BoomererangState.NONE;
        rb = GetComponent<Rigidbody>();
        GetComponent<MeshCollider>().enabled = false;
    }

    void Update()
    {
        if (transform.parent == null)
        {
            GetComponent<MeshCollider>().enabled = true;
            rb.isKinematic = false;
            switch (boomererangState)
            {
                case BoomererangState.THROW:
                    if (elapsed > 5)
                    {
                        elapsed = 0;
                        boomererangState = BoomererangState.RECOIL;
                    }
                    rb.velocity = dir * speed * Time.deltaTime;
                    break;
                case BoomererangState.RECOIL:
                    rb.velocity = (creator.transform.position - rb.position).normalized * speed * Time.deltaTime;
                    break;
            }
            elapsed += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        FPS ownerofBoom = collider.gameObject.GetComponent<FPS>();
        EntityBase entity = collider.gameObject.GetComponent<EntityBase>();


        if (ownerofBoom == null && entity == null)
        {
            boomererangState = BoomererangState.RECOIL;
        }
        else
        {
            if (entity.gameObject != creator)
            {
                Vector3 dir = entity.transform.position - transform.position;
                entity.TakeDamage(damage, dir);
            }

            if (ownerofBoom != null)
            {
                if (ownerofBoom.gameObject == creator)
                {
                    if (boomererangState == BoomererangState.RECOIL)
                    {
                        transform.parent = boomerangWeapon.transform.GetChild(0);
                        transform.localPosition = Vector3.zero;
                        transform.localRotation = Quaternion.identity;
                        GetComponent<MeshCollider>().enabled = false;
                        rb.isKinematic = true;
                        rb.velocity = Vector3.zero;
                        boomererangState = BoomererangState.NONE;
                    }
                }
            }
        }
    }
}
