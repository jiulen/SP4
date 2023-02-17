using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormBlast : ProjectileBase
{
    protected GameObject playerManager;
    bool active = true;
    Vector3 direction;
    Vector3 pos;

    void Awake()
    {
        playerManager = GameObject.Find("Player Manager");

    }

    void Update()
    {
        if (!active)
        {
            Debug.Log("POS:" + pos);
            Debug.Log(direction);
            Debug.DrawRay(pos, direction *10000, Color.blue);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active)
            return;
        Debug.Log("SEX");

        Debug.Log(creator.name);
        Debug.Log(other.gameObject.name);
        if (other.gameObject == creator)
            return;

        foreach(Transform child in playerManager.transform)
        {
            pos = transform.position;
            direction = child.Find("Player Entity").transform.position - transform.position;
            direction.Normalize();
            child.transform.GetChild(0).GetComponent<Rigidbody>().AddForce(direction * 1000);
            child.transform.GetChild(0).GetComponent<Rigidbody>().AddForce(0,100,0);

        }
        active = false;
        //Destroy(gameObject);
    }
}
