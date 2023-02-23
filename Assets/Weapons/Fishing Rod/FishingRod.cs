using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : WeaponBase
{
    [SerializeField] GameObject fishingLine; //line goes from bulletEmitter(line pivot) to hook pivot (when hook pivot is lauched away from line pivot)
    [SerializeField] Transform hookPivot;
    [SerializeField] Transform hookParent; //Gun

    LineRenderer fishingLineRenderer;

    float fishingRodPitch = 0f;
    const float fishingRodPitchSpeed = 500f;

    const float fishingThrowSpeed = 30f;
    const float fishingPullForce = 10f; //Force to launch hook and pull player back

    public Rigidbody hookedRigidbody = null;

    public Vector3 hookVelocity = Vector3.zero;

    public enum HookState
    {
        INACTIVE,
        NOT_HOOKED,
        HOOKED
    };
    public HookState hookState = HookState.INACTIVE;

    [SerializeField] AudioSource rodPullAudio;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        fishingLineRenderer = fishingLine.GetComponent<LineRenderer>();
        fishingLineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (hookState != HookState.INACTIVE)
        {
            //Draw fishing line
            fishingLineRenderer.SetPosition(0, bulletEmitter.transform.position);
            fishingLineRenderer.SetPosition(1, hookPivot.position);

            //Swing fishing rod down
            if (fishingRodPitch < 0)
                fishingRodPitch = Mathf.MoveTowards(fishingRodPitch, 0, fishingRodPitchSpeed * Time.deltaTime);
        }
        else
        {
            //Swing fishing rod up
            if (fishingRodPitch > -55)
                fishingRodPitch = Mathf.MoveTowards(fishingRodPitch, -55, fishingRodPitchSpeed * Time.deltaTime);
        }

        hookParent.localRotation = Quaternion.Euler(fishingRodPitch, 0, 0);
    }

    override protected void Fire1Once() //no cooldown for fishing rod
    {
        if (hookState == HookState.INACTIVE)
        {
            //Raycast front
            //Throw hook out
            RaycastHit raycastHitFront;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out raycastHitFront, 200))
            {
                hookVelocity = (raycastHitFront.point - bulletEmitter.transform.position).normalized * fishingThrowSpeed;
            }
            else
            {
                hookVelocity = (camera.transform.position + camera.transform.forward * 10 - bulletEmitter.transform.position).normalized * fishingThrowSpeed;
            }            

            hookState = HookState.NOT_HOOKED;
            fishingLine.SetActive(true);
            hookPivot.parent = null;
            hookedRigidbody = null;

            fireAudio.Play();
        }
        else
        {
            if (hookState == HookState.HOOKED && hookedRigidbody)
            {
                hookedRigidbody.AddForce((hookPivot.position - hookedRigidbody.position).normalized * fishingPullForce, ForceMode.Impulse);
            }

            //Teleport hook back
            hookPivot.position = bulletEmitter.transform.position;

            hookVelocity = Vector3.zero;
            hookState = HookState.INACTIVE;
            hookPivot.parent = hookParent; //Set hook parent back to fishing rod
            hookPivot.localRotation = Quaternion.identity;
            fishingLine.SetActive(false);
            hookedRigidbody = null;

            rodPullAudio.Play();
        }
    }
}
