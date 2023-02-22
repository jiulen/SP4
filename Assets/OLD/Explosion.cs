using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Explosion : ProjectileBase
{
    private AudioSource explodeAudio;
    public AudioClip clip;
    public float radius, explosionForce;

    private void Start()
    {
        explodeAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {

    }

    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            EntityBase entity = collider.gameObject.GetComponent<EntityBase>();
            if (entity != null)
            {
                //if (entity.gameObject != creator) // if the explosion is not the creator
                //{
                    Vector3 dir = entity.transform.position - transform.position;
                    entity.TakeDamage(damage, dir);
                //}
            }
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
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}
