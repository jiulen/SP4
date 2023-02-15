using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCollectibles : MonoBehaviour
{
    [SerializeField] Healthbar healthmanager;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Collectibles")
        {
            if (collision.gameObject.name == "HealthPack(Clone)")
            {
                healthmanager.SetHealth(healthmanager.GetHealth() + 20f);
            }
            else if (collision.gameObject.name == "Ammo(Clone)")
            {

            }
            Destroy(collision.gameObject);
        }
    }
}
