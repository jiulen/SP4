using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public float fireRate = 1;
    public int damage = 1;
    public int bulletsPerShot = 1;
    public float inaccuracy = 10;
    public float projectileVel = 1;

    protected Rigidbody rb;
    public GameObject weaponholder;
    protected Camera cam;
    bool Equipped = true;
    protected double elapsedSinceLastShot = 0;
    protected double elapsedBetweenEachShot = 0;

    void Start()
    { 
        elapsedBetweenEachShot = 1 / fireRate;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateWeaponBase()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Equipped)
                Drop();
            else
                Equip();
        }
    }
    public void Equip()
    {
        transform.SetParent(weaponholder.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);

        rb.isKinematic = true;
        Equipped = true;
    }
    public void Drop()
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.velocity = rb.GetComponent<Rigidbody>().velocity;
        rb.AddForce(cam.transform.forward * 200f * Time.deltaTime, ForceMode.Impulse);

        Equipped = false;
    }
    public static Vector3 RandomSpray(Vector3 front, float maxInnacuracy)
    {
        float randomAngle = Random.Range(-maxInnacuracy / 2, maxInnacuracy / 2);
        Vector3 finalVector = Quaternion.AngleAxis(randomAngle, Vector3.right) * front;
        randomAngle = Random.Range(-maxInnacuracy / 2, maxInnacuracy / 2);

        return Quaternion.AngleAxis(randomAngle, Vector3.up) * finalVector;
    }

}
