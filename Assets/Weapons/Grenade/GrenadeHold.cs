using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GrenadeHold : WeaponBase
{
    private float Cooldown = 5f, currentCooldownElaspe = 0f;
    public float ThrowForce;
    [SerializeField] GameObject grenade;
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
        SpawnGrenadeWeaponHoldServerRpc();

    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnGrenadeWeaponHoldServerRpc()
    {
        grenade = Instantiate(grenadePrefab, new Vector3(0,0,0), Quaternion.identity);
        grenade.GetComponent<GrenadeProjectile>().SetCollider(false);
        grenade.GetComponent<GrenadeProjectile>().damage = damage[0];
        grenade.GetComponent<NetworkObject>().Spawn();
        grenade.GetComponent<NetworkObject>().TrySetParent(gameObject);
        grenade.GetComponent<ProjectileBase>().SetWeaponUsed(this.gameObject);
        grenade.GetComponent<GrenadeProjectile>().SetObjectReferencesClientRpc(owner.GetComponent<NetworkObject>().NetworkObjectId,
                                                                               particleManager.GetComponent<NetworkObject>().NetworkObjectId);
        grenade.transform.localPosition = Vector3.zero;
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
                    SpawnGrenadeWeaponHoldServerRpc();
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
            AudioThrow.Play();
            ThrowServerRpc();
            SetExplosionstateServerRpc();
            grenadestate = GrenadeWeaponState.THROW;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetExplosionstateServerRpc()
    {
        if (grenadestate == GrenadeWeaponState.NONE)
        {
            grenade.GetComponent<GrenadeProjectile>().state = GrenadeProjectile.GrenadeState.EXPLODE;

        }
    }

    [ServerRpc(RequireOwnership = false)]
    void ThrowServerRpc()
    {
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        grenade.GetComponent<GrenadeProjectile>().SetCollider(true);
        grenade.GetComponent<ProjectileBase>().SetWeaponUsed(this.gameObject);
        grenade.GetComponent<GrenadeProjectile>().SetObjectReferencesClientRpc(owner.GetComponent<NetworkObject>().NetworkObjectId,
                                                                               particleManager.GetComponent<NetworkObject>().NetworkObjectId);
        grenade.transform.SetParent(projectileManager.transform);
        rb.AddForce(camera.transform.forward * ThrowForce, ForceMode.Impulse);
    }


}
