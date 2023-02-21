using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : WeaponBase
{
    [SerializeField] GameObject fishingLine; //line goes from bulletEmitter(line pivot) to hook pivot (when hook pivot is lauched away from line pivot)
    [SerializeField] Transform hookPivot;
    [SerializeField] Transform hookParent;

    const float fishingThrowSpeed = 20f;
    const float fishingPullForce = 100f; //Force to launch hook and pull player back

    public Vector3 hookVelocity = Vector3.zero;

    public enum HookState
    {
        INACTIVE,
        NOT_HOOKED,
        HOOKED
    };
    public HookState hookState = HookState.INACTIVE;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    override protected void Fire1Once() //no cooldown for fishing rod
    {
        if (hookState == HookState.INACTIVE)
        {
            //Swing fishing rod down


            //Throw hook out
            Vector3 shootDir = camera.transform.forward * 100 - bulletEmitter.transform.position;
            hookVelocity = shootDir.normalized * fishingThrowSpeed;

            hookState = HookState.NOT_HOOKED;
            fishingLine.SetActive(true);
            hookPivot.parent = null;
        }
        else
        {
            //Swing fishing rod up


            //Teleport hook back
            hookPivot.position = bulletEmitter.transform.position;

            hookVelocity = Vector3.zero;
            hookState = HookState.INACTIVE;
            hookPivot.parent = hookParent; //Set hook parent back to fishing rod
            fishingLine.SetActive(false);

            if (hookState == HookState.HOOKED)
            {
                //If hooked player, pull player
            }
        }
    }
}
