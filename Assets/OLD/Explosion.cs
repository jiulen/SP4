using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
            if (collider.gameObject.tag == "PlayerHitBox")
            {
                EntityBase entity = collider.GetComponent<PlayerHitBox>().owner.GetComponent<EntityBase>();
                Vector3 dir = entity.transform.position - transform.position;
                entity.TakeDamage(damage, dir);
            }
            else
            {
                EntityBase entity = collider.GetComponent<EntityBase>();
                if (entity != null)
                {
                    Vector3 dir = entity.transform.position - transform.position;
                    entity.TakeDamage(damage, dir);
                }
            }
        }

        Collider[] collidersmove = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in collidersmove)
        {
            if (collider.gameObject.tag == "PlayerHitBox")
            {
                Rigidbody rb = collider.GetComponent<PlayerHitBox>().owner.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, transform.position, radius);
                }
            }
            else
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, transform.position, radius);
                }
            }
        }
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}
