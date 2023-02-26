using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Rocket : ProjectileBase
{

    [ClientRpc]
    private void SetVelocityClientRpc(Vector3 velocity)
    {
        GetComponent<Rigidbody>().velocity = velocity;
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
        elapsed += Time.deltaTime;
        if (elapsed >= duration)
        {
            RocketExplode();
            Destroy(gameObject);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject != creator)
        {
            RocketExplode();
            Destroy(this.gameObject);
        }
    }

    private void RocketExplode()
    {
        explosion.Explode();
        particleManager.GetComponent<ParticleManager>().CreateEffect("Explosion_PE", this.transform.position, this.transform.forward);
    }

}
