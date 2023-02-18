using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Explosion : MonoBehaviour
{
    private AudioSource explodeAudio;
    public AudioClip clip;
    public float radius, explosionForce;
    public float damage;
    private void Start()
    {
        explodeAudio = GetComponent<AudioSource>();
    }
    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            EntityBase entity = collider.gameObject.GetComponent<EntityBase>();
            if (entity != null)
            {
                Vector3 dir = entity.transform.position - transform.position;
                entity.TakeDamage(damage, dir);
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