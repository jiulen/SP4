using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Boomerang : ProjectileBase
{
    private float rotationElaspe;
    public float rotationSpeed;
    private float speed;
    private Rigidbody rb;
    public Vector3 dir;
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
        speed = 4000f;
        damage = boomerangWeapon.damage[0];
        rb = GetComponent<Rigidbody>();
        boomererangState = BoomererangState.NONE;
    }

    void Update()
    {
        if (transform.parent == null)
        {
            transform.forward = dir.normalized;
            switch (boomererangState)
            {
                case BoomererangState.THROW:
                    if (speed <= 0)
                    {
                        boomererangState = BoomererangState.RECOIL;
                    }
                    rb.velocity = transform.forward * -speed * Time.deltaTime;
                    break;
                case BoomererangState.RECOIL:
                    dir = (rb.position - boomerangWeapon.emitter.transform.position).normalized;
                    rb.velocity = transform.forward * speed * Time.deltaTime;
                    break;
            }
            transform.localRotation = Quaternion.LookRotation(-transform.forward);
            var axis = transform.InverseTransformDirection(transform.up);
            transform.localRotation = transform.localRotation * Quaternion.AngleAxis(rotationElaspe * rotationSpeed, axis);

            rotationElaspe += Time.deltaTime;
            speed -= Time.deltaTime * 2000f;

            speed = Mathf.Clamp(speed, -4000f, 4000f);

            elapsed += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        bool temp = false;
        if (boomererangState != BoomererangState.NONE)
        {
            PlayerHitBox myplayer = collider.gameObject.GetComponent<PlayerHitBox>();

            if (myplayer != null)
            {
                if (creator == myplayer.owner)
                {
                    if (boomererangState == BoomererangState.RECOIL)
                    {
                        transform.parent = boomerangWeapon.transform.GetChild(0);
                        transform.localPosition = Vector3.zero;
                        transform.localRotation = Quaternion.identity;
                        GetComponent<MeshCollider>().enabled = false;
                        rb.isKinematic = true;
                        rb.velocity = Vector3.zero;
                        speed = 4000f;
                        elapsed = 0;
                        boomererangState = BoomererangState.NONE;
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
}
