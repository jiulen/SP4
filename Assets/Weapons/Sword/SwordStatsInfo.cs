using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordStatsInfo : MonoBehaviour
{
    private int damage;

    Sword sword;
    private void Start()
    {
        sword = transform.parent.parent.GetComponent<Sword>();
        damage = sword.damage[0];
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (sword.GetAnimator().GetBool("TrSlice"))
            sword.OnChildTriggerEnter(other, damage);
    }
}
