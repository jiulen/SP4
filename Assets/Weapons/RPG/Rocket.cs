using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Rocket : ProjectileBase
{
    public NetworkVariable<Vector3> LaserVelocity = new NetworkVariable<Vector3>();

    public override void OnNetworkSpawn()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(LaserVelocity.Value.x, LaserVelocity.Value.y, LaserVelocity.Value.z);
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
        transform.rotation = Quaternion.LookRotation(transform.forward);
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
