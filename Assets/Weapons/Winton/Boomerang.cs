using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : ProjectileBase
{
    public Vector3 dir;
    private float rotationSpeed;
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
        rotationSpeed = 100f;
        boomererangState = BoomererangState.NONE;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (transform.parent == null)
        {
            switch(boomererangState)
            {
                case BoomererangState.THROW:
                    if (elapsed > 1)
                    {
                        elapsed = 0;
                        boomererangState = BoomererangState.RECOIL;
                    }
                    rb.velocity = dir * Time.deltaTime;
                    break;
                case BoomererangState.RECOIL:
                    rb.velocity = (creator.transform.position - rb.position).normalized * 100 * Time.deltaTime;
                    break;
            }
            elapsed += Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (boomererangState != BoomererangState.NONE && collision.gameObject != creator)
            boomererangState = BoomererangState.RECOIL;

        if (boomererangState == BoomererangState.RECOIL && collision.gameObject == creator)
        {
            transform.parent = boomerangWeapon.gameObject.transform;
            boomererangState = BoomererangState.NONE;
            elapsed = 0;
        }
    }
}
