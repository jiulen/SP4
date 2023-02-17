using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float radius, explosionForce;
    public float damage;
    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            EntityBase entity = collider.gameObject.GetComponent<EntityBase>();
            if (entity != null)
                entity.TakeDamage(damage);
        }

        Collider[] collidersmove = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in collidersmove)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, radius);
            }
        }
    }
}
