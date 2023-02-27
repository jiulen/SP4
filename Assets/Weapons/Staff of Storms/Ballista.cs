using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

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
            PlayAudioServerRpc();

            fireAnimation.enabled = true;
            fireAnimation.StopPlayback();
            fireAnimation.Play("Fire1");

            fireAnimation.speed = 1 / (float)elapsedBetweenEachShot[0];

            Ray laserRayCast = new Ray(camera.transform.position + camera.transform.forward * 0.5f, camera.transform.forward);

            int hitType = -1;
            if (Physics.Raycast(laserRayCast, out RaycastHit hit, 1000))
            {
                Vector3 direction = hit.point - bulletEmitter.transform.position;
                

                if(hit.collider.transform.tag == "PlayerHitBox")
                {
                    EntityBase player = hit.collider.gameObject.GetComponent<PlayerHitBox>().owner.GetComponent<EntityBase>();

                    if (player.gameObject == owner)
                    {
                        Ray laserRayCast2 = new Ray(hit.point + camera.transform.forward * 0.1f, camera.transform.forward);
                        if (Physics.Raycast(laserRayCast2, out RaycastHit hit2, 1000))
                        {
                            if (hit.collider.transform.tag == "PlayerHitBox")
                            {
                                EntityBase player2 = hit2.collider.gameObject.GetComponent<PlayerHitBox>().owner.GetComponent<EntityBase>();

                                if (hit2.collider.name == "Head")
                                {
                                    hitType = 2;
                                    player2.TakeDamage(damage[0] * 2, camera.transform.forward, owner, this.gameObject);
                                }
                                else
                                {
                                    hitType = 3;
                                    player2.TakeDamage(damage[0], camera.transform.forward, owner, this.gameObject);
                                }
                                CreateLaserServerRpc(hit2.point, hit2.normal, hitType, bulletEmitter.transform.position, camera.transform.forward, hit2.distance);
                            }
                            else
                            {
                                hitType = 1;
                                CreateLaserServerRpc(hit2.point, hit2.normal, hitType, bulletEmitter.transform.position, camera.transform.forward, hit2.distance);
                            }
                        }
                        else
                        {
                            hitType = 0;

                            EntityBase entity = hit.transform.gameObject.GetComponent<EntityBase>();
                            if (entity != null)
                            {
                                entity.TakeDamage(damage[0], camera.transform.forward, owner, gameObject);
                            }

                            CreateLaserServerRpc(Vector3.zero, Vector3.zero, hitType, bulletEmitter.transform.position, camera.transform.forward, 1000);
                        }
                    }
                    else
                    {
                        if (hit.collider.name == "Head")
                        {
                            hitType = 2;
                            player.TakeDamage(damage[0] * 2, camera.transform.forward, owner, this.gameObject);
                            
                        }
                        else
                        {
                            hitType = 3;
                            player.TakeDamage(damage[0], camera.transform.forward, owner, this.gameObject);

                        }
                        CreateLaserServerRpc(hit.point, hit.normal, hitType, bulletEmitter.transform.position, camera.transform.forward, hit.distance);
                    }
                }
                else
                {
                    hitType = 1;

                    EntityBase entity = hit.transform.gameObject.GetComponent<EntityBase>();
                    if (entity != null)
                    {
                        entity.TakeDamage(damage[0], camera.transform.forward, owner, gameObject);
                    }

                    CreateLaserServerRpc(hit.point, hit.normal, hitType, bulletEmitter.transform.position, camera.transform.forward, hit.distance);
                }
            }
            else
            {
                hitType = 0;
                CreateLaserServerRpc(Vector3.zero, Vector3.zero, hitType, bulletEmitter.transform.position, camera.transform.forward, 1000);
            }

            Vector3 knockBackDirection = camera.transform.forward;
            knockBackDirection.y = 0;
            knockBackDirection.Normalize();
            owner.GetComponent<Rigidbody>().AddForce(-knockBackDirection * fire1KnockbackForce);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateLaserServerRpc(Vector3 hitPoint, Vector3 hitNormal, int hitType, Vector3 startPoint, Vector3 direction, float distance) //Calls the ClientRpc function
    {
        CreateLaserClientRpc(hitPoint, hitNormal, hitType, startPoint, direction, distance);
    }

    [ClientRpc]
    private void CreateLaserClientRpc(Vector3 hitPoint, Vector3 hitNormal, int hitType, Vector3 startPoint, Vector3 direction, float distance)
    {
        GameObject laser = Instantiate(ballistaLaserPF, startPoint, Quaternion.identity);
        BallistaLaser laserScript = laser.GetComponent<BallistaLaser>();

        if (hitType >= 1)
            laserScript.InitParticleSystem(startPoint, direction, distance);
        else
            laserScript.InitParticleSystem(startPoint, direction, distance);
                                           
        //Particle effects
        switch (hitType)
        {
            case 2:
                particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hitPoint, hitNormal, 45);
                break;
            case 3:
                particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hitPoint, hitNormal);
                break;
        }

        //Explosion
        if (hitType >= 1 && hitType <= 3)
            particleManager.GetComponent<ParticleManager>().CreateEffect("ElectricExplosion_PE", hitPoint, hitNormal);
    }

    override protected void Fire2Once()
    {
        if (CheckCanFire(2))
        {
            PlayAudio2ServerRpc();

            Transform newTransform = camera.transform;
            Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
            CreateBlastServerRpc(bulletEmitter.transform.position);

            fireAnimation.enabled = true;
            fireAnimation.StopPlayback();
            fireAnimation.Play("Fire2");

            fireAnimation.speed = 1 / (float)elapsedBetweenEachShot[0];
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateBlastServerRpc(Vector3 startPos)
    {
        GameObject go = Instantiate(stormBlastPF, startPos, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
        go.GetComponent<NetworkObject>().TrySetParent(projectileManager);
        go.GetComponent<ProjectileBase>().SetWeaponUsed(this.gameObject);
        go.GetComponent<StormBlast>().damage = damage[0];
        go.GetComponent<StormBlast>().SetObjectReferencesClientRpc(owner.GetComponent<NetworkObject>().NetworkObjectId,
                                                                   particleManager.GetComponent<NetworkObject>().NetworkObjectId);
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayAudioServerRpc()
    {
        PlayAudioClientRpc();
    }

    [ClientRpc]
    void PlayAudioClientRpc()
    {
        AudioFire1.Play();
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayAudio2ServerRpc()
    {
        PlayAudio2ClientRpc();
    }

    [ClientRpc]
    void PlayAudio2ClientRpc()
    {
        AudioFire2.Play();
    }
} 
