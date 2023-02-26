using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class Boomerang : ProjectileBase
{
    private float rotationElaspe;
    public float rotationSpeed;
    private float speed;
    private Rigidbody rb;
    public Vector3 dir;
    public enum BoomererangState
    {
        THROW,
        RECOIL,
    }
    public BoomererangState boomererangState;

    void Start()
    {
        speed = 5000f;
        rb = GetComponent<Rigidbody>();
        boomererangState = BoomererangState.THROW;
    }

    void Update()
    {
        UpdateBoomerangServerRpc();
    }

    [ServerRpc(RequireOwnership =false)]
    void UpdateBoomerangServerRpc()
    {
        UpdateBoomerangClientRpc();
    }

    [ClientRpc]
    void UpdateBoomerangClientRpc()
    {
        transform.forward = dir.normalized;
        switch (boomererangState)
        {
            case BoomererangState.THROW:
                if (speed <= 0)
                {
                    boomererangState = BoomererangState.RECOIL;
                }
                SetVelocity(transform.forward * -speed * Time.deltaTime);
                break;
            case BoomererangState.RECOIL:
                dir = (rb.position - creator.transform.position).normalized;
                SetVelocity(transform.forward * speed * Time.deltaTime);
                break;
        }
        transform.localRotation = Quaternion.LookRotation(-transform.forward);
        var axis = transform.InverseTransformDirection(transform.up);
        transform.localRotation = transform.localRotation * Quaternion.AngleAxis(rotationElaspe * rotationSpeed, axis);

        rotationElaspe += Time.deltaTime;
        speed -= Time.deltaTime * 2800f;

        speed = Mathf.Clamp(speed, -5000f, 5000f);

        elapsed += Time.deltaTime;
    }


    private void OnTriggerEnter(Collider collider)
    {
        bool temp = false;

        PlayerHitBox myplayer = collider.gameObject.GetComponent<PlayerHitBox>();

        if (myplayer != null)
        {
            if (creator == myplayer.owner)
            {
                if (boomererangState == BoomererangState.RECOIL)
                {
                    Destroy(this.gameObject);
                }
            }
            else
            {
                temp = true;
            }
        }
        else
        {
            temp = true;
        }





        if (temp)
        {
            if (collider.tag == "PlayerHitBox")
            {
                EntityBase player = collider.gameObject.GetComponent<PlayerHitBox>().owner.GetComponent<EntityBase>();
                Vector3 dir = transform.position - player.transform.position;

                Vector3 vel = this.GetComponent<Rigidbody>().velocity;
                debugOnTriggerBackwardsPosition = this.transform.position - vel.normalized * 1;
                Ray laserRayCast = new Ray(debugOnTriggerBackwardsPosition, vel);
                if (Physics.Raycast(laserRayCast, out RaycastHit hit, 1))
                {
                    if (collider.name == "Head")
                    {
                        particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal, 15);
                        player.TakeDamage(damage * 2, dir, creator, weaponused);
                    }
                    else
                    {
                        particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal);
                        player.TakeDamage(damage, dir, creator, weaponused);
                    }
                }
            }
            else
            {
                EntityBase entity = collider.gameObject.GetComponent<EntityBase>();
                if (entity != null)
                {
                    Vector3 dir = transform.position - entity.transform.position;
                    entity.TakeDamage(damage, dir, creator, weaponused);
                }
                else
                {
                    if (Physics.Raycast(rb.position, transform.forward, out RaycastHit hit))
                        particleManager.GetComponent<ParticleManager>().CreateEffect("Sparks_PE", rb.position, hit.normal);

                    if (boomererangState == BoomererangState.THROW)
                    {
                        speed = 0;
                        boomererangState = BoomererangState.RECOIL;
                    }
                }
            }
        }
       

    }
}
