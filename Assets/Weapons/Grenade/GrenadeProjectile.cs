using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GrenadeProjectile : ProjectileBase
{
    private Explosion explosion;


    // Start is called before the first frame update
    void Start()
    {
        explosion = GetComponent<Explosion>();
        explosion.SetCreator(creator);
        explosion.SetWeaponUsed(weaponused);
        explosion.damage = damage;
        duration = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= duration)
            {
                GrenadeExplode();
                Destroy(gameObject);
            }
        }
    }

    private void GrenadeExplode()
    {
        explosion.Explode();

        MakeExplosionEffectServerRpc(transform.position, transform.forward);
    }

    protected void OnTriggerEnter(Collider other) //override projectile base - do nothing
    {

    }

    [ServerRpc(RequireOwnership = false)]
    private void MakeExplosionEffectServerRpc(Vector3 position, Vector3 normal)
    {
        MakeExplosionEffectClientRpc(position, normal);
    }

    [ClientRpc]
    private void MakeExplosionEffectClientRpc(Vector3 position, Vector3 normal)
    {
        Debug.LogWarning("KABOOM");
        particleManager.GetComponent<ParticleManager>().CreateEffect("Explosion_PE", position, normal);
    }

    [ClientRpc]
    private void SetVelocityClientRpc(Vector3 velocity)
    {
        GetComponent<Rigidbody>().AddForce(velocity, ForceMode.Impulse);
    }

    [ServerRpc]
    private void SetVelocityServerRpc(Vector3 velocity)
    {
        SetVelocityClientRpc(velocity);
    }

    public void SetVelocity(Vector3 velocity)
    {
        SetVelocityServerRpc(velocity);
    }
}
