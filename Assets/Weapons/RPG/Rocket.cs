using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Rocket : ProjectileBase
{
    private Explosion explosion;
    // Start is called before the first frame update
    void Start()
    {
        explosion = GetComponent<Explosion>();
        explosion.damage = damage;
        explosion.SetCreator(creator);
        explosion.SetWeaponUsed(weaponused);
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(-GetComponent<Rigidbody>().velocity);
        if (IsServer)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= duration)
            {
                RocketExplode();
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            if (collision != null && collision.gameObject != creator)
            {
                RocketExplode();
                Destroy(this.gameObject);
            }
        }
    }

    private void RocketExplode()
    {
        explosion.Explode();

        MakeExplosionEffectServerRpc(transform.position, transform.forward);
    }

    [ServerRpc (RequireOwnership = false)]
    private void MakeExplosionEffectServerRpc(Vector3 position, Vector3 normal)
    {
        MakeExplosionEffectClientRpc(position, normal);
    }

    [ClientRpc]
    private void MakeExplosionEffectClientRpc(Vector3 position, Vector3 normal)
    {
        particleManager.GetComponent<ParticleManager>().CreateEffect("Explosion_PE", position, normal);
    }
}
