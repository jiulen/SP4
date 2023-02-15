using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjects : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject destructcrates, Health, Ammo;
    private Vector3 offset;
    private Rigidbody rb;
    void Start()
    {
        offset = new Vector3(0, 0, 0);
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            switch(Random.Range(0, 2)) // 0: none
            {
                case 1:
                    Instantiate(Health, rb.position + offset, Quaternion.identity);
                    break;
                case 2:
                    Instantiate(Ammo, rb.position + offset, Quaternion.identity);
                    break;
            }
            Destroy(this.gameObject);
            Instantiate(destructcrates, rb.position, Quaternion.identity);
        }
    }

}
