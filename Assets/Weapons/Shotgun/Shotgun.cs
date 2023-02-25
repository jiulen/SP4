using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class Shotgun : WeaponBase
{
    [SerializeField] TrailRenderer bulletTrail;
    const float trailSpeed = 200f;

    void Start()
    {
        base.Start();
    }

    override protected void Fire1Once()
    {
        if (CheckCanFire(1))
        {
            Transform newTransform = camera.transform;
            int hitType = -1;
            for (int bullets = 0; bullets < bulletsPerShot[0]; ++bullets)
            {
                Vector3 bulletDir = newTransform.forward;
                bulletDir = RandomSpray(bulletDir.normalized, inaccuracy[0]);
                //Do hitscan
                RaycastHit hit;
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

                                    if (hit2.collider.name == "Head")
                                    {
                                        hitType = 2;
                                        player2.TakeDamage(damage[0] * 2, -bulletDir, owner, this.gameObject);
                                    }
                                    else
                                    {
                                        hitType = 3;
                                        player2.TakeDamage(damage[0] * 2, -bulletDir, owner, this.gameObject);
                                    }

                                    CreateTrailServerRpc(hit2.point, hit2.normal, hitType);
                                }
                                else
                                {
                                    hitType = 1;

                                    EntityBase entity2 = hit2.transform.gameObject.GetComponent<EntityBase>();
                                    if (entity2 != null)
                                    {
                                        entity2.TakeDamage(damage[0], -bulletDir, owner, this.gameObject);
                                    }

                                    CreateTrailServerRpc(hit2.point, hit2.normal, hitType);
                                }
                            }
                            else
                            {
                                hitType = 0;
                                CreateTrailServerRpc(bulletEmitter.transform.position + bulletDir * 200, Vector3.zero, hitType);
                            }
                        }
                        else
                        {
                            if (hit.collider.name == "Head")
                            {
                                hitType = 2;
                                player.TakeDamage(damage[0] * 2, -bulletDir, owner, this.gameObject);
                            }
                            else
                            {
                                hitType = 3;
                                player.TakeDamage(damage[0] * 2, -bulletDir, owner, this.gameObject);
                            }

                            CreateTrailServerRpc(hit.point, hit.normal, hitType);
                        }
                    }
                    else
                    {
                        hitType = 1;

                        EntityBase entity = hit.transform.gameObject.GetComponent<EntityBase>();
                        if (entity != null)
                        {
                            entity.TakeDamage(damage[0], -bulletDir, owner, this.gameObject);
                        }

                        CreateTrailServerRpc(hit.point, hit.normal, hitType);
                    }

                }
                else
                {
                    hitType = 0;

                    CreateTrailServerRpc(bulletEmitter.transform.position + bulletDir * 200, Vector3.zero, hitType);
                }
            }
            fireAudio.Play();
        }
    }

    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint)
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

    [ServerRpc (RequireOwnership = false)]
    private void CreateTrailServerRpc(Vector3 hitPoint, Vector3 hitNormal, int hitType) //Calls the ClientRpc function
    {
        CreateTrailClientRpc(hitPoint, hitNormal, hitType);
    }

    [ClientRpc]
    private void CreateTrailClientRpc(Vector3 hitPoint, Vector3 hitNormal, int hitType) //hitType: 0 - no hit, 1 - hit terrain, 2 - hit player head, 3 - hit player body
    {
        //Make trail renderer
        TrailRenderer trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);

        //Spawn bullet tracer
        StartCoroutine(SpawnTrail(trail, hitPoint));

        //Hit particle effects
        switch (hitType)
        {
            case 1:
                particleManager.GetComponent<ParticleManager>().CreateEffect("Sparks_PE", hitPoint, hitNormal);
                break;
            case 2:
                particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hitPoint, hitNormal, 15);
                break;
            case 3:
                particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hitPoint, hitNormal);
                break;
        }
    }
}
