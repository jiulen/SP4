using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCATBullet : ProjectileBase
{
    void Start()
    {
        
    }

    void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CheckIfCreator(other.gameObject))
            return;

        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        debugOnTriggerBackwardsPosition = this.transform.position - vel.normalized * 1;
        Ray laserRayCast = new Ray(debugOnTriggerBackwardsPosition, vel);
        if (Physics.Raycast(laserRayCast, out RaycastHit hit, 1))
        {
            //Debug.LogError(hit.collider.name);
            GameObject effect;
            if (hit.collider.tag == "PlayerHitBox")
            {
                effect = Instantiate(bloodEffect, particleManager.transform);
                if(hit.collider.name == "Head")
                {
                    ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
                    particleSystem.Stop();
                    var burst = particleSystem.emission;
                    ParticleSystem.Burst newBurst = new ParticleSystem.Burst(0, 15);
                    burst.SetBurst(0, newBurst);
                    particleSystem.Play();
                }
            
                //hit.transform.GetComponent<PlayerHitBox>().owner.GetComponent<PlayerEntity>().TakeDamage(damage, new Vector3(0,0,1)); //Temporarily set as forward
            }
            else
            {
                effect = Instantiate(sparkEffect, particleManager.transform);


            }

            effect.transform.position = hit.point;
            effect.transform.rotation = Quaternion.LookRotation(hit.normal);


            //Debug.LogError(DebugSavePosition);

            //float dotProduct = Vector3.Dot(vel, hit.normal);
            //Vector3 reflectionVector = 2 * dotProduct * hit.normal;
            //Vector3 reflectedVector = vel - reflectionVector;
            //this.GetComponent<Rigidbody>().velocity = reflectedVector.normalized * this.GetComponent<Rigidbody>().velocity.magnitude;
        }

        //this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //this.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);

        Destroy(gameObject);

    }
}
