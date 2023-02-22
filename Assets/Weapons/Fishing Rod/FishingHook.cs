using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingHook : MonoBehaviour
{
    [SerializeField] FishingRod fishingRod;

    ParticleManager particleManager;

    float gravity = -9.81f;

    private void Start()
    {
        particleManager = GameObject.Find("Particle Manager").GetComponent<ParticleManager>();
    }

    private void FixedUpdate()
    {
        //Move hook if detached
        if (fishingRod.hookState == FishingRod.HookState.NOT_HOOKED)
        {
            //Gravity
            fishingRod.hookVelocity.y += gravity * Time.fixedDeltaTime;

            transform.position += fishingRod.hookVelocity * Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fishingRod.hookState == FishingRod.HookState.NOT_HOOKED)
        {
            transform.position = other.ClosestPointOnBounds(transform.position);

            //Check what is hooked
            if (other.tag == "PlayerHitBox")
            {
                if (other.name == "Head")
                {
                    particleManager.CreateEffect("Blood_PE", transform.position, -fishingRod.hookVelocity.normalized, 15);
                }
                else
                {
                    particleManager.CreateEffect("Blood_PE", transform.position, -fishingRod.hookVelocity.normalized);

                }
                fishingRod.hookedRigidbody = other.attachedRigidbody;
            }
            else
                particleManager.CreateEffect("Sparks_PE", transform.position, -fishingRod.hookVelocity.normalized);

            fishingRod.hookState = FishingRod.HookState.HOOKED;
        }
    }
}