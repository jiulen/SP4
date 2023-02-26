using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BoomerangWeapon : WeaponBase
{
    [SerializeField] GameObject boomerang;
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
                if (boomerang.gameObject == null)
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
            Transform newTransform = camera.transform;
            Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
            ThrowBoomeramgServerRpc(front, bulletEmitter.transform.position);
            boomererangWeaponState = BoomererangWeaponState.THROW;
        }
    }


    [ServerRpc(RequireOwnership = false)]
    void ThrowBoomeramgServerRpc(Vector3 front, Vector3 spawnposition)
    {
        GameObject go = Instantiate(boomerang, spawnposition, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
        go.GetComponent<NetworkObject>().TrySetParent(projectileManager);
        go.GetComponent<ProjectileBase>().SetWeaponUsed(this.gameObject);
        go.GetComponent<Boomerang>().damage = damage[0];
        go.GetComponent<Boomerang>().SetObjectReferencesClientRpc(owner.GetComponent<NetworkObject>().NetworkObjectId,
                                                               particleManager.GetComponent<NetworkObject>().NetworkObjectId);
        go.GetComponent<Boomerang>().dir = front;
    }
}
