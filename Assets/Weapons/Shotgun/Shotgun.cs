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

    void Start()
    {
        base.Start();
        bulletEmitter = GameObject.Find("Gun/BulletEmitter");
    }

    override protected void Fire1Once()
    {
        if (CheckCanFire(1))
        {
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
                    //Damage stuff (Edit later)
                    EntityBase entity = hit.transform.gameObject.GetComponent<EntityBase>();
                    if (entity != null)
                    {
                        Vector3 dir = -bulletDir;
                        entity.TakeDamage(1, dir);
                    }

                    //Do bullet tracer (if hit)
                    trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);
                    //Spawn bullet tracer
                    StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, hit.collider, true));
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
            fireAudio.Play();
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

        if (madeImpact && hitCollider)
        {
            //do hit effects here
        }

        Destroy(trail.gameObject, trail.time);
    }
}
