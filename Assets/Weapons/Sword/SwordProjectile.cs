using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordProjectile : ProjectileBase
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
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
            if (hit.collider.tag == "PlayerHitBox")
            {
                if (hit.collider.name == "Head")
                {
                    particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal, 15);
                }
                else
                {
                    particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal);

                }
            }
            else
            {
                particleManager.GetComponent<ParticleManager>().CreateEffect("Sparks_PE", hit.point, hit.normal);
            }

        }
    }
}
