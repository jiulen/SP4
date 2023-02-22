using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : WeaponBase
{
    private Vector3 shootVelocity;
    private float timetotravel, currenttimeelaspe = 0;
    enum BoomererangState
    {
        NONE,
        THROW,
        RECOIL
    }
    BoomererangState boomererangState;

    // Start is called before the first frame update
    void Start()
    {
        timetotravel = 0.9f;
        boomererangState = BoomererangState.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        switch(boomererangState)
        {
            case BoomererangState.THROW:

                if (currenttimeelaspe > timetotravel)
                {
                    currenttimeelaspe = 0;
                    boomererangState = BoomererangState.RECOIL;
                }
                currenttimeelaspe += Time.deltaTime;
                break;
            case BoomererangState.RECOIL:
                break;
        }
    }

    protected override void Fire1Once()
    {
        if (boomererangState == BoomererangState.NONE)
        {
            shootVelocity = camera.transform.forward * 100;
            transform.parent = null;
            boomererangState = BoomererangState.THROW;
        }
    }
}
