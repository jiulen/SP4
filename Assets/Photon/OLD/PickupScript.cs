using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickupScript : MonoBehaviour
{
    Transform transform;
    double elapsedTime;
    Vector3 originalPos;
    public string weaponName;
    PhotonView photonView;
    void Start()
    {
        transform = GetComponent<Transform>();
        elapsedTime = 0;
        originalPos = transform.position;
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        transform.position = new Vector3(originalPos.x, originalPos.y + Mathf.Sin((float) elapsedTime), originalPos.z);
        transform.eulerAngles = new Vector3(0, (float)elapsedTime * 30.0f, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("COLLIDE!");

        if (collision.gameObject.tag == "Player")
        {
            PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
            player.ChangeWeapon(weaponName);
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Destroy(gameObject);
        }
    }
}
