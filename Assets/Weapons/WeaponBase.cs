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

    //protected Rigidbody rb;
    //public GameObject weaponholder;
    [SerializeField] protected Camera cam;
    //bool Equipped = true;
    protected double elapsedSinceLastShot = 0;
    protected double elapsedBetweenEachShot = 0;

    protected AudioSource fireAudio;
    protected Camera camera;
    protected GameObject projectileManager;
    protected GameObject bulletEmitter;

    public void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        elapsedBetweenEachShot = 1 / fireRate;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        fireAudio = GameObject.Find("SampleFire").GetComponent<AudioSource>();
        projectileManager = GameObject.Find("Projectile Manager");
        bulletEmitter = GameObject.Find("Bullet emitter");

    }

    // Update is called once per frame
    public void Update()
    {
        elapsedSinceLastShot += Time.deltaTime;

        if (Input.GetButton("Fire1"))
        {
            Fire1();
        }

        if (Input.GetButton("Fire2"))
        {
            Fire2();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Fire1Once();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Fire2Once();
        }
    }

    protected virtual void Fire1()
    {

    }

    protected virtual void Fire2()
    {

    }

    protected virtual void Fire1Once()
    {

    }

    protected virtual void Fire2Once()
    {

    }

    //public void UpdateWeaponBase()
    //{

    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        if (Equipped)
    //            Drop();
    //        else
    //            Equip();
    //    }
    //}
    //public void Equip()
    //{
    //    transform.SetParent(weaponholder.transform);
    //    transform.localPosition = Vector3.zero;
    //    transform.localRotation = Quaternion.Euler(Vector3.zero);

    //    rb.isKinematic = true;
    //    Equipped = true;
    //}
    //public void Drop()
    //{
    //    transform.SetParent(null);
    //    rb.isKinematic = false;
    //    rb.velocity = rb.GetComponent<Rigidbody>().velocity;
    //    rb.AddForce(cam.transform.forward * 300f * Time.deltaTime, ForceMode.Impulse);

    //    Equipped = false;
    //}
    public static Vector3 RandomSpray(Vector3 front, float maxInnacuracy)
    {
        float randomAngle = Random.Range(-maxInnacuracy / 2, maxInnacuracy / 2);
        Vector3 finalVector = Quaternion.AngleAxis(randomAngle, Vector3.right) * front;
        randomAngle = Random.Range(-maxInnacuracy / 2, maxInnacuracy / 2);

        return Quaternion.AngleAxis(randomAngle, Vector3.up) * finalVector;
    }

}
