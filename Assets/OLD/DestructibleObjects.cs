using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjects : EntityBase
{
    // Start is called before the first frame update
    public GameObject destructcrates, HealthPack, Ammo;
    private Vector3 offset;
    private Rigidbody rb;
   
    void Start()
    {
        offset = new Vector3(0, 0, 0);
        rb = GetComponent<Rigidbody>();
        base.Start();
    }

    public void DestroyDestructible()
    {
        switch(Random.Range(0, 2)) // 0: none
        {
            case 1:
                Instantiate(HealthPack, rb.position + offset, Quaternion.identity);
                break;
            case 2:
                Instantiate(Ammo, rb.position + offset, Quaternion.identity);
                break;
        }
        if (this.gameObject.tag == "Crates")
        {
            Instantiate(destructcrates, rb.position, Quaternion.identity);
        }
        Destroy(this.gameObject);
    }

    void Update()
    {
        base.Update();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }
    override public void TakeDamage(float hp)
    {
        SetHealth(GetHealth() - hp);

        if (Health <= 0)
        {
            DestroyDestructible();
        }
    }
}
