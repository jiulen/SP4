using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public int damage = 0;
    public float duration = 1;

    protected double elapsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        Debug.Log(elapsed);
        if(elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag == "Player")
        //{ 
        //    PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        //    player.ReceiveDamage(damage);
        //    Debug.Log("DAMAGE!" + damage);
        //}
        ////if (PhotonNetwork.IsMasterClient)
        //if(!(collision.gameObject.tag=="Bullet"))
        //    PhotonNetwork.Destroy(gameObject);
    }
}
