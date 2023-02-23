using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangWeapon : WeaponBase
{
    [SerializeField] Transform boomerang;
    public float spinRate;

    enum BoomererangWeaponState
    {
        NONE,
        THROW
    }
    BoomererangWeaponState boomererangWeaponState;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        boomererangWeaponState = BoomererangWeaponState.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        switch (boomererangWeaponState)
        {
            case BoomererangWeaponState.THROW:
                if (boomerang.parent != null)
                {
                    boomererangWeaponState = BoomererangWeaponState.NONE;
                }
                break;
        }

    }

    protected override void Fire1Once()
    {
        if (boomererangWeaponState == BoomererangWeaponState.NONE)
        {
            Rigidbody rb = boomerang.GetComponent<Rigidbody>();
            boomerang.GetComponent<ProjectileBase>().SetObjectReferences(owner, particleManager);
            boomerang.GetComponent<Boomerang>().boomererangState = Boomerang.BoomererangState.THROW;
            boomerang.GetComponent<MeshCollider>().enabled = true;
            boomerang.GetComponent<Boomerang>().dir = -camera.transform.forward;
            rb.isKinematic = false;
            boomerang.parent = null;
            boomererangWeaponState = BoomererangWeaponState.THROW;
        }
    }
}
