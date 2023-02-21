using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingHook : MonoBehaviour
{
    [SerializeField] FishingRod fishingRod;

    float gravity = -9.81f;

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
            Debug.Log("Hooked");
            //Check what is hooked

            fishingRod.hookState = FishingRod.HookState.HOOKED;
        }
    }
}