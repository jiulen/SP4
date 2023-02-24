using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingHook : MonoBehaviour
{
    [SerializeField] FishingRod fishingRod;

    ParticleManager particleManager;

    [SerializeField] AudioSource hookHitAudio;

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
                EntityBase player = other.gameObject.GetComponent<PlayerHitBox>().owner.GetComponent<EntityBase>();

                if (other.name == "Head")
                {
                    particleManager.CreateEffect("Blood_PE", transform.position, -fishingRod.hookVelocity.normalized, 15);
                    player.TakeDamage(fishingRod.hookDamage * 2, -fishingRod.hookVelocity.normalized, fishingRod.GetOwner(), fishingRod.gameObject);
                }
                else
                {
                    particleManager.CreateEffect("Blood_PE", transform.position, -fishingRod.hookVelocity.normalized);
                    player.TakeDamage(fishingRod.hookDamage, -fishingRod.hookVelocity.normalized, fishingRod.GetOwner(), fishingRod.gameObject);

                }
                fishingRod.hookedRigidbody = other.attachedRigidbody;
                fishingRod.hookedCollider = other;
                hookHitAudio.Play();
            }
            else
            {
                particleManager.CreateEffect("Sparks_PE", transform.position, -fishingRod.hookVelocity.normalized);
                EntityBase entity = other.gameObject.GetComponent<EntityBase>();

                if (entity != null)
                {
                    entity.TakeDamage(fishingRod.hookDamage, -fishingRod.hookVelocity.normalized, fishingRod.GetOwner(), fishingRod.gameObject);
                }
            }

            fishingRod.hookState = FishingRod.HookState.HOOKED;
            transform.parent = other.transform;
        }
    }
}