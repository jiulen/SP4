using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ExplosiveBullet : ProjectileBase
{
    protected GameObject playerManager;

    // Anything outisde this range will not be affected
    public float upperExplosionRadius = 10;

    // A set force will be given to anything within this range. Anything outside of it will be given a variable force based on distance
    public float innerExplosionRadius = 1;

    public float explosionForce = 1000;

    // Base y axis force applied to the player if they are grounded
    public float verticalExplosionForce = 100;

    public float armingDistance = 10;
    public float maxDistance = 20;
    protected float traveledDistance = 0;

    void Awake()
    {
        playerManager = GameObject.Find("Player Manager");
        //particleManager = GameObject.Find("Particle Manager");
    }

    void Update()
    {
        traveledDistance += this.GetComponent<Rigidbody>().velocity.magnitude * Time.deltaTime;
        if (traveledDistance >= maxDistance)
            Explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CheckIfCreator(other.gameObject))
            return;
        if(traveledDistance < armingDistance)
        {
            Destroy(gameObject);
            return;
        }
        Explode();
    }

    private void Explode()
    {
        foreach (Transform child in playerManager.transform)
        {
            Vector3 difference = child.transform.position - transform.position;
            float distance = difference.magnitude;
            if (distance > upperExplosionRadius)
            {
                continue;
            }

            Vector3 direction = difference.normalized;

            Rigidbody childRB = child.transform.GetComponent<Rigidbody>();
            FPS childFPS = child.transform.GetComponent<FPS>();

            if (distance < innerExplosionRadius)
            {
                childRB.AddForce(direction * explosionForce);

                // We lift the player up so they are unnaffliced by ground friction
                if (childFPS.GetIsGrounded())
                    childRB.AddForce(0, verticalExplosionForce, 0);
            }
            else
            {
                float distancedExplosionForce = explosionForce / distance; // We do not square distance as per the inverse square law, as it would be too weak
                childRB.AddForce(direction * distancedExplosionForce);

                // We lift the player up so they are unnaffliced by ground friction
                if (childFPS.GetIsGrounded())
                    childRB.AddForce(0, verticalExplosionForce, 0);

            }


        }

        MakeExplosionEffectServerRpc(transform.position, Vector3.up);

        Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
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
