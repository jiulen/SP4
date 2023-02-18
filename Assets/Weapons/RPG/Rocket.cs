using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : ProjectileBase
{
    private Explosion explosion;
    // Start is called before the first frame update
    void Start()
    {
        explosion = GetComponent<Explosion>();
        explosion.damage = damage;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.forward);
        base.Update();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject != creator)
        {
            explosion.Explode();
            Destroy(this.gameObject);
        }
    }
}
