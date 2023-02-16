using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormBlast : ProjectileBase
{
    protected GameObject playerManager;

    void Awake()
    {
        playerManager = GameObject.Find("Player Manager");

    }

    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        foreach(Transform child in playerManager.transform)
        {
            
            Vector3 direction = child.transform.position - transform.position;
            direction.Normalize();
            direction.y += 0.01f;
            child.transform.GetChild(0).GetComponent<Rigidbody>().AddForce(direction * 10000);
        }
        Destroy(gameObject);
    }
}
