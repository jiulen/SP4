using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BoomerangWeapon : WeaponBase
{
    [SerializeField] GameObject boomerang;
    GameObject boom;

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
        UpdateboomStatusServerRpc();
    }

    protected override void Fire1Once()
    {
        if (boomererangWeaponState == BoomererangWeaponState.NONE)
        {
            Transform newTransform = camera.transform;
            Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
            ThrowBoomeramgServerRpc(front, bulletEmitter.transform.position);
            SetBoomModelActiveServerRpc(false);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    void ThrowBoomeramgServerRpc(Vector3 front, Vector3 spawnposition)
    {
        if (boom == null)
        {
            boom = Instantiate(boomerang, spawnposition, Quaternion.identity);
            boom.GetComponent<NetworkObject>().Spawn();
            boom.GetComponent<NetworkObject>().TrySetParent(projectileManager);
            boom.GetComponent<ProjectileBase>().SetWeaponUsed(this.gameObject);
            boom.GetComponent<Boomerang>().damage = damage[0];
            boom.GetComponent<Boomerang>().SetObjectReferencesClientRpc(owner.GetComponent<NetworkObject>().NetworkObjectId,
                                                                 particleManager.GetComponent<NetworkObject>().NetworkObjectId);
            boom.GetComponent<Boomerang>().dir = -front;
            boomererangWeaponState = BoomererangWeaponState.THROW;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateboomStatusServerRpc()
    {
        UpdateboomStatusClientRpc();
    }
    [ClientRpc]
    void UpdateboomStatusClientRpc()
    {
        switch (boomererangWeaponState)
        {
            case BoomererangWeaponState.THROW:
                if (boom == null)
                {
                    boomererangWeaponState = BoomererangWeaponState.NONE;
                    SetBoomModelActiveServerRpc(true);
                }
                break;
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void SetBoomModelActiveServerRpc(bool active)
    {
        SetBoomModelActiveClientRpc(active);
    }

    [ClientRpc]
    private void SetBoomModelActiveClientRpc(bool active)
    {
        weaponModel.SetActive(active);
    }
}
