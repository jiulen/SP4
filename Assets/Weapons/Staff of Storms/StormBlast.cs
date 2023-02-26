using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StormBlast : ProjectileBase
{
    protected GameObject playerManager;

    // Anything outisde this range will not be affected
    public float upperExplosionRadius = 10;

    // A set force will be given to anything within this range. Anything outside of it will be given a variable force based on distance
    public float innerExplosionRadius = 1;

    public float explosionForce = 1000;

    // Base y axis force applied to the player if they are grounded
    public float verticalExplosionForce = 100;

    void Awake()
    {
        playerManager = GameObject.Find("Player Manager");
        particleManager = GameObject.Find("Particle Manager");
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CheckIfCreator(other.gameObject))
            return;
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
                if(childFPS.GetIsGrounded())
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
        particleManager.GetComponent<ParticleManager>().CreateEffect("WindExplosion_PE", this.transform.position, Vector3.up);

        Destroy(gameObject);
    }
}


// This version of storm blast is designed for debugging. It won't destroy the game object, and will draw a debug ray showing the direction of the blast to the player

//public class StormBlast : ProjectileBase
//{
//    protected GameObject playerManager;
//    bool active = true;
//    Vector3 direction;
//    Vector3 pos;

//    void Awake()
//    {
//        playerManager = GameObject.Find("Player Manager");

//    }

//    void Update()
//    {
//        if (!active)
//        {
//            Debug.Log("POS:" + pos);
//            Debug.Log(direction);
//            Debug.DrawRay(pos, direction *10000, Color.blue);
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!active)
//            return;
//        Debug.Log("SEX");

//        Debug.Log(creator.name);
//        Debug.Log(other.gameObject.name);
//        if (other.gameObject == creator)
//            return;

//        foreach(Transform child in playerManager.transform)
//        {
//            pos = transform.position;
//            direction = child.Find("Player Entity").transform.position - transform.position;
//            direction.Normalize();
//            child.transform.GetChild(0).GetComponent<Rigidbody>().AddForce(direction * 1000);
//            child.transform.GetChild(0).GetComponent<Rigidbody>().AddForce(0,100,0);

//        }
//        active = false;
//        //Destroy(gameObject);
//    }
//}
