using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Photon.Pun;

public class PlayerCharacter : MonoBehaviour
{

    Vector3 movement;
    Rigidbody rigidbody;
    Transform transform;

    GameObject target;
    GameObject healthBar;
    GameObject gun;

    public GameObject SMGReference;
    public GameObject SniperReference;
    public GameObject ShotgunReference;

    AudioSource audio;

    double elapsedSinceLastFired;
    int maxHealth = 100;
    int health = 100;

    bool dodgeRollActive = false;
    Vector3 dodgeDirection = new Vector3(0, 0, 0);
    double elapsedSinceDodge;
    float dodgeDuration = 0.2f;
    bool lose = false;
    //private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        //test push
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ChangeWeapon(string weapon)
    {
    }

      
}
