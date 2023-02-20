using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordProjectile : ProjectileBase
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotation = Quaternion.LookRotation(transform.forward);
        transform.rotation = rotation;
    }
}
