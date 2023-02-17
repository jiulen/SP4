using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCollectibles : MonoBehaviour
{
    private EntityBase player;

    private void Start()
    {
        player = GetComponent<EntityBase>();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Collectibles")
        {
            if (collision.gameObject.name == "HealthPack(Clone)")
            {
                player.SetHealth(player.GetHealth() + 20f);
            }
            else if (collision.gameObject.name == "Ammo(Clone)")
            {

            }
            Destroy(collision.gameObject);
        }
    }
}
