using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileBase : NetworkBehaviour
{
    public int damage = 0;
    public float duration = 1;

    protected double elapsed = 0;
    protected GameObject creator;
    protected GameObject particleManager;

    public GameObject bloodEffect;
    public GameObject sparkEffect;

    protected GameObject weaponused;
    public void SetWeaponUsed(GameObject obj)
    {
        weaponused = obj;
    }

    // Used to debug particle effect spawn location using ray tracing. Comment out destroy and debug.drawray to use
    protected Vector3 debugOnTriggerBackwardsPosition;
    public void SetCreator(GameObject _creator)
    {
        creator = _creator;
    }

    public void SetParticleManager(GameObject _particleManager)
    {
        particleManager = _particleManager;
    }

    [ClientRpc]
    public void SetObjectReferencesClientRpc(ulong _creatorID, ulong _particleManagerID)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(_creatorID, out NetworkObject creator) &&
            NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(_particleManagerID, out NetworkObject particleManager))
        {
            SetObjectReferences(creator.gameObject, particleManager.gameObject);
        }
        else Debug.LogWarning("SetObjRefCliRpc Fail!");
    }

    public void SetObjectReferences(GameObject _creator, GameObject _particleManager)
    {
        creator = _creator;
        particleManager = _particleManager;
    }

    public void Start()
    {
    }

    public void Update()
    {
        //Debug.DrawRay(this.transform.position, (debugOnTriggerBackwardsPosition - this.transform.position), Color.green);
        elapsed += Time.deltaTime;
        if(elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
     
    }

    // Checks if a collision with a hitbox that belongs to a player, and returns true if player is same as the one who created that projectile
    public bool CheckIfCreator(GameObject other)
    {

        if (other.tag == "PlayerHitBox")
        {
            GameObject hitBoxOwner = other.GetComponent<PlayerHitBox>().owner;
            //Debug.LogWarning("HIT" + hitBoxOwner + "GAY" + creator);

            if (hitBoxOwner == creator)
                return true;
            return false;
        }
        return false;
    }

    protected void OnTriggerEnter(Collider other)
    {
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        debugOnTriggerBackwardsPosition = this.transform.position - vel.normalized * 1;
        Ray laserRayCast = new Ray(debugOnTriggerBackwardsPosition, vel);
        if (Physics.Raycast(laserRayCast, out RaycastHit hit, 1))
        {
            GameObject effect = Instantiate(sparkEffect, particleManager.transform);
            effect.transform.position = hit.point;
            //Debug.LogError(DebugSavePosition);
            effect.transform.rotation = Quaternion.LookRotation(hit.normal);

            //float dotProduct = Vector3.Dot(vel, hit.normal);
            //Vector3 reflectionVector = 2 * dotProduct * hit.normal;
            //Vector3 reflectedVector = vel - reflectionVector;
            //this.GetComponent<Rigidbody>().velocity = reflectedVector.normalized * this.GetComponent<Rigidbody>().velocity.magnitude;
        }

        //this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //this.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);


        //Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        //Ray laserRayCast = new Ray(this.transform.position + (-vel.normalized * 0.1f), vel);
        //if (Physics.Raycast(laserRayCast, out RaycastHit hit, 50))
        //{
        //    GameObject effect = Instantiate(sparkEffect, particleManager.transform);
        //    effect.transform.position = this.transform.position;
        //    effect.transform.rotation = Quaternion.Euler(hit.normal);
        //}

        EntityBase entity = other.gameObject.GetComponent<EntityBase>();
        if (entity != null)
        {
            Vector3 dir = entity.transform.position - transform.position;
            entity.TakeDamage(damage, -dir, creator, weaponused);
        }

        Destroy(gameObject);

    }

    [ClientRpc]
    private void SetVelocityClientRpc(Vector3 velocity)
    {
        GetComponent<Rigidbody>().velocity = velocity;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetVelocityServerRpc(Vector3 velocity)
    {
        SetVelocityClientRpc(velocity);
    }

    public void SetVelocity(Vector3 velocity)
    {
        SetVelocityServerRpc(velocity);
    }
}
