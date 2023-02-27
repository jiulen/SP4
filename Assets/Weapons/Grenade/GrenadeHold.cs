using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GrenadeHold : WeaponBase
{
    private float Cooldown = 5f, currentCooldownElaspe = 0f;
    public float ThrowForce;
    [SerializeField] GameObject grenadePrefab;

    public AudioSource AudioThrow;

    enum GrenadeWeaponState
    {
        NONE,
        THROW
    }
    GrenadeWeaponState grenadestate;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        grenadestate = GrenadeWeaponState.NONE;

    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        switch (grenadestate)
        {
            case GrenadeWeaponState.THROW:
                if (currentCooldownElaspe >= Cooldown)
                {
                    currentCooldownElaspe = 0;
                    SetKnifeModelActiveServerRpc(true);
                    grenadestate = GrenadeWeaponState.NONE;
                }
                currentCooldownElaspe += Time.deltaTime;
                break;
        }
    }

    protected override void Fire1Once()
    {
        if (grenadestate == GrenadeWeaponState.NONE)
        {
            PlayAudioServerRpc();
            Transform newTransform = camera.transform;
            Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
            ThrowServerRpc(front, bulletEmitter.transform.position);
            grenadestate = GrenadeWeaponState.THROW;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void ThrowServerRpc(Vector3 front, Vector3 spawnposition)
    {
        GameObject grenade = Instantiate(grenadePrefab, spawnposition, Quaternion.identity);
        grenade.GetComponent<NetworkObject>().Spawn();
        grenade.GetComponent<ProjectileBase>().damage = damage[0];
        grenade.GetComponent<NetworkObject>().TrySetParent(projectileManager);
        grenade.GetComponent<ProjectileBase>().SetWeaponUsed(this.gameObject);
        grenade.GetComponent<GrenadeProjectile>().SetObjectReferencesClientRpc(owner.GetComponent<NetworkObject>().NetworkObjectId,
                                                                               particleManager.GetComponent<NetworkObject>().NetworkObjectId);
        grenade.GetComponent<GrenadeProjectile>().SetVelocity(front.normalized * ThrowForce);
        SetKnifeModelActiveServerRpc(false);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SetKnifeModelActiveServerRpc(bool active)
    {
        SetKnifeModelActiveClientRpc(active);
    }

    [ClientRpc]
    private void SetKnifeModelActiveClientRpc(bool active)
    {
        weaponModel.SetActive(active);
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayAudioServerRpc()
    {
        PlayAudioClientRpc();
    }

    [ClientRpc]
    void PlayAudioClientRpc()
    {
        AudioThrow.Play();
    }
}
