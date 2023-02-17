using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public int damage = 0;
    public float duration = 1;

    protected double elapsed = 0;
    protected GameObject creator;

    public void SetCreator(GameObject _creator)
    {
        creator = _creator;
    }
    public void Start()
    {
    }

    public void Update()
    {
        elapsed += Time.deltaTime;
        if(elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
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
