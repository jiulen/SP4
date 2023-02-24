using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (other.tag == "PlayerHitBox")
            {
                EntityBase player = other.gameObject.GetComponent<PlayerHitBox>().owner.GetComponent<EntityBase>();
                Vector3 dir = player.transform.position - transform.position;

                if (player.gameObject != creator)
                {
                    if (other.name == "Head")
                    {
                        particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal, 15);
                        player.TakeDamage(damage * 2, -dir, creator, weaponused);

                    }
                    else
                    {
                        particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal);
                        player.TakeDamage(damage, -dir, creator, weaponused);

                    }
                }
            }
            else
            {
                particleManager.GetComponent<ParticleManager>().CreateEffect("Sparks_PE", hit.point, hit.normal);
                EntityBase entity = other.gameObject.GetComponent<EntityBase>();

                if (entity != null)
                {
                    Vector3 dir = entity.transform.position - transform.position;
                    entity.TakeDamage(damage, dir, creator, weaponused);
                }
            }

        }

        Destroy(gameObject);

    }
}
