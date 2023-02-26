using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SwordProjectile : ProjectileBase
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        duration = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        Quaternion rotation = Quaternion.LookRotation(transform.forward);
        transform.rotation = rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        debugOnTriggerBackwardsPosition = this.transform.position - vel.normalized * 1;
        Ray laserRayCast = new Ray(debugOnTriggerBackwardsPosition, vel);
        if (Physics.Raycast(laserRayCast, out RaycastHit hit, 1))
        {
            Debug.LogWarning("Name : " + other.name);
            if (other.tag == "PlayerHitBox")
            {
                EntityBase player = other.gameObject.GetComponent<PlayerHitBox>().owner.GetComponent<EntityBase>();
                Vector3 dir = player.transform.position - transform.position;
                if (player.gameObject != creator)
                {
                    if (other.name == "Head")
                    {
                        SpawnHitParticleServerRpc(hit.point, hit.normal, 2);
                        player.TakeDamage(damage * 2, dir, creator, weaponused);

                    }
                    else
                    {
                        SpawnHitParticleServerRpc(hit.point, hit.normal, 3);
                        player.TakeDamage(damage, dir, creator, weaponused);

                    }
                    Destroy(gameObject);
                }
            }
            else
            {
                SpawnHitParticleServerRpc(hit.point, hit.normal, 1);
                EntityBase entity = other.gameObject.GetComponent<EntityBase>();

                if (entity != null)
                {
                    Vector3 dir = entity.transform.position - transform.position;
                    entity.TakeDamage(damage, dir, creator, weaponused);
                }

                Destroy(gameObject);
            }
        }
    }

    [ServerRpc]
    private void SpawnHitParticleServerRpc(Vector3 hitPoint, Vector3 hitNormal, int hitType)
    {
        SpawnHitParticleClientRpc(hitPoint, hitNormal, hitType);
    }

    [ClientRpc]
    private void SpawnHitParticleClientRpc(Vector3 hitPoint, Vector3 hitNormal, int hitType)
    {
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
