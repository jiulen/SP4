using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public int damage = 0;
    public float duration = 1;

    protected double elapsed = 0;
    protected GameObject creator;
    protected GameObject particleManager;

    public GameObject bloodEffect;
    public GameObject sparkEffect;

    public void SetCreator(GameObject _creator)
    {
        creator = _creator;
    }

    public void SetProjectileManager(GameObject pm)
    {
        particleManager = pm;
    }

    public void Start()
    {
    }

    public void Update()
    {
        elapsed += Time.deltaTime;
        if(elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }

    // Checks if a collision with a hitbox that belongs to a player, and returns true if player is same as the one who created that projectile
    public bool CheckIfCreator(GameObject other)
    {
        Debug.LogError("HIT" + other.transform.name);

        if (other.tag == "PlayerHitBox")
        {
            GameObject hitBoxOwner = other.GetComponent<PlayerHitBox>().owner;

            if (hitBoxOwner == creator)
                return true;
            return false;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        //Ray laserRayCast = new Ray(this.transform.position + (-vel.normalized * 0.1f), vel);
        //if (Physics.Raycast(laserRayCast, out RaycastHit hit, 50))
        //{
        //    GameObject effect = Instantiate(sparkEffect, particleManager.transform);
        //    effect.transform.position = this.transform.position;
        //    effect.transform.rotation = Quaternion.Euler(hit.normal);
        //}
        Destroy(gameObject);
    }

}
