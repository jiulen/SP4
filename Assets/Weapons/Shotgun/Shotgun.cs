using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shotgun : WeaponBase
{
    //[SerializeField] ParticleSystem shootingSystem;
    //[SerializeField] ParticleSystem impactParticleSystem;
    [SerializeField] TrailRenderer bulletTrail;
    const float trailSpeed = 200f;

    [Header("Audio References")]
    public AudioSource AudioFire1;
    public AudioSource AudioFire2;

    void Start()
    {
        base.Start();
        fireAnimation = weaponModel.transform.Find("Bennelli_M4").GetComponent<Animator>();

    }

    override protected void Fire1Once()
    {
        if (CheckCanFire(1))
        {
            AudioFire1.Play();
            fireAnimation.speed = 1 / (float)elapsedBetweenEachShot[0];
            fireAnimation.PlayInFixedTime("Fire", -1, 0);
            muzzleFlash.GetComponent<ParticleSystem>().Stop();
            muzzleFlash.GetComponent<ParticleSystem>().Play();

            Transform newTransform = camera.transform;
            for (int bullets = 0; bullets < bulletsPerShot[0]; ++bullets)
            {
                Vector3 bulletDir = newTransform.forward;
                bulletDir = RandomSpray(bulletDir.normalized, inaccuracy[0]);
                //Do hitscan
                RaycastHit hit;
                TrailRenderer trail = null;
                if (Physics.Raycast(newTransform.position, bulletDir, out hit))
                {
                    if (hit.collider.tag == "PlayerHitBox")
                    {
                        EntityBase player = hit.collider.gameObject.GetComponent<PlayerHitBox>().owner.GetComponent<EntityBase>();

                        if (player.gameObject == owner)
                        {
                            //Do second hitscan
                            RaycastHit hit2;
                            if (Physics.Raycast(hit.point + bulletDir * 0.1f, bulletDir, out hit2))
                            {
                                if (hit2.collider.tag == "PlayerHitBox")
                                {
                                    EntityBase player2 = hit2.collider.gameObject.GetComponent<PlayerHitBox>().owner.GetComponent<EntityBase>();
                                    //Shouldnt need to check for own player again

                                    //Do bullet tracer (if hit)
                                    trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);
                                    //Spawn bullet tracer
                                    StartCoroutine(SpawnTrail(trail, hit2.point, hit2.normal, hit2.collider, true));

                                    if (hit2.collider.name == "Head")
                                    {
                                        particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit2.point, hit2.normal, 15);
                                        player2.TakeDamage(damage[0] * 2, -bulletDir, owner, this.gameObject);
                                    }
                                    else
                                    {
                                        particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit2.point, hit2.normal);
                                        player2.TakeDamage(damage[0] * 2, -bulletDir, owner, this.gameObject);
                                    }
                                }
                                else
                                {
                                    //Do bullet tracer (if hit)
                                    trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);
                                    //Spawn bullet tracer
                                    StartCoroutine(SpawnTrail(trail, hit2.point, hit2.normal, hit2.collider, true));

                                    particleManager.GetComponent<ParticleManager>().CreateEffect("Sparks_PE", hit2.point, hit2.normal);

                                    EntityBase entity2 = hit2.transform.gameObject.GetComponent<EntityBase>();
                                    if (entity2 != null)
                                    {
                                        entity2.TakeDamage(damage[0], -bulletDir, owner, this.gameObject);
                                    }
                                }
                            }
                            else
                            {
                                //Do bullet tracer (if no hit)
                                trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);

                                //Spawn bullet tracer
                                StartCoroutine(SpawnTrail(trail, bulletEmitter.transform.position + bulletDir * 200, Vector3.zero, null, false));
                            }
                        }
                        else
                        {
                            //Do bullet tracer (if hit)
                            trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);
                            //Spawn bullet tracer
                            StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, hit.collider, true));

                            if (hit.collider.name == "Head")
                            {
                                particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal, 15);
                                player.TakeDamage(damage[0] * 2, -bulletDir, owner, this.gameObject);
                            }
                            else
                            {
                                particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal);
                                player.TakeDamage(damage[0] * 2, -bulletDir, owner, this.gameObject);
                            }
                        }
                    }
                    else
                    {
                        //Do bullet tracer (if hit)
                        trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);
                        //Spawn bullet tracer
                        StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, hit.collider, true));

                        particleManager.GetComponent<ParticleManager>().CreateEffect("Sparks_PE", hit.point, hit.normal);

                        EntityBase entity = hit.transform.gameObject.GetComponent<EntityBase>();
                        if (entity != null)
                        {
                            entity.TakeDamage(damage[0], -bulletDir, owner, this.gameObject);
                        }
                    }

                }
                else
                {
                    //Do bullet tracer (if no hit)
                    trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);

                    //Spawn bullet tracer
                    StartCoroutine(SpawnTrail(trail, bulletEmitter.transform.position + bulletDir * 200, Vector3.zero, null, false));
                }
            }
            //shootingSystem.Play();
            AudioFire1.Play();
        }
    }

    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal, Collider hitCollider, bool madeImpact)
    {
        Vector3 startPos = trail.transform.position;

        float distance = Vector3.Distance(trail.transform.position, hitPoint);
        float startDistance = distance;

        while (distance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPos, hitPoint, 1 - (distance / startDistance));
            distance -= Time.deltaTime * trailSpeed;

            yield return null;
        }

        trail.transform.position = hitPoint;

        Destroy(trail.gameObject, trail.time);
    }
}
