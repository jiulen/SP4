using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballista : WeaponBase
{
    public GameObject ballistaLaserPF;
    public GameObject stormBlastPF;

    public float fire1KnockbackForce = 1;

    [Header("Audio References")]
    public AudioSource AudioFire1;
    public AudioSource AudioFire2;

    void Start()
    {
        base.Start();
        bulletEmitter = GameObject.Find("Gun/Bullet emitter");

        fireAnimation = weaponModel.transform.Find("Staff").GetComponent<Animator>();

    }

    void Update()
    {
        base.Update();
    }

    override protected void Fire1Once()
    {
        if (CheckCanFire(1))
        {
            AudioFire1.Play();

            fireAnimation.enabled = true;
            fireAnimation.StopPlayback();
            fireAnimation.Play("Fire1");

            fireAnimation.speed = 1 / (float)elapsedBetweenEachShot[0];

            Ray laserRayCast = new Ray(camera.transform.position + camera.transform.forward * 0.5f, camera.transform.forward);
            GameObject laser = Instantiate(ballistaLaserPF, projectileManager.transform);
            BallistaLaser laserScript = laser.GetComponent<BallistaLaser>();
            
            if (Physics.Raycast(laserRayCast, out RaycastHit hit, 1000))
            {
                
                Vector3 direction = hit.point - bulletEmitter.transform.position;
                laserScript.InitParticleSystem(bulletEmitter.transform.position, direction, hit.distance);
                if(hit.collider.transform.tag == "PlayerHitBox")
                {
                    if (hit.collider.name == "Head")
                    {
                        particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal, 45);
                    }
                    else
                    {
                        particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal);

                    }
                    particleManager.GetComponent<ParticleManager>().CreateEffect("ElectricExplosion_PE", hit.point, hit.normal);

                }
                else
                {
                    particleManager.GetComponent<ParticleManager>().CreateEffect("ElectricExplosion_PE", hit.point, hit.normal);

                }
            }
            else
            {
                //Vector3 test = bulletEmitter.transform.position;
                //test = camera.transform.position;
                //test = laser.transform.position;
                Vector3 direction = camera.transform.forward * 1000 - bulletEmitter.transform.position;
                laserScript.InitParticleSystem(bulletEmitter.transform.position, camera.transform.forward, 1000);

            }
            Debug.DrawRay(camera.transform.position, 50 * (camera.transform.forward), Color.blue);

            Vector3 knockBackDirection = camera.transform.forward;
            knockBackDirection.y = 0;
            knockBackDirection.Normalize();
            owner.GetComponent<Rigidbody>().AddForce(-knockBackDirection * fire1KnockbackForce);
        }

    }

    override protected void Fire2Once()
    {
        if (CheckCanFire(2))
        {
            AudioFire2.Play();
            Transform newTransform = camera.transform;
            GameObject blast = Instantiate(stormBlastPF, bulletEmitter.transform);
            Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
            blast.GetComponent<Rigidbody>().velocity = front.normalized * projectileVel[1];
            blast.transform.SetParent(projectileManager.transform);
            blast.GetComponent<StormBlast>().SetCreator(owner);
            AudioFire2.Play();

            fireAnimation.enabled = true;
            fireAnimation.StopPlayback();
            fireAnimation.Play("Fire2");

            fireAnimation.speed = 1 / (float)elapsedBetweenEachShot[0];

        }
    }

} 
