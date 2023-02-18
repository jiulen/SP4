using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public float[] fireRate;
    public int[] damage;
    public int[] bulletsPerShot;
    public float[] inaccuracy;
    public float[] projectileVel;
    //protected Rigidbody rb;
    //public GameObject weaponholder;

    //bool Equipped = true;
    protected double[] elapsedSinceLastShot;
    protected double[] elapsedBetweenEachShot;

    protected AudioSource fireAudio;
    protected Camera camera;
    protected GameObject projectileManager;
    protected GameObject bulletEmitter;
    public Sprite WeaponIcon;
    protected GameObject playerOwner;
    private bool isLeftClickDown = false, isRightClickDown = false;

    public void Start()
    {
        //elapsedBetweenEachShot = 1 / fireRate;
        elapsedSinceLastShot = new double[fireRate.Length];
        elapsedBetweenEachShot = new double[fireRate.Length];
        for (int i =0; i != fireRate.Length; i++)
        {
            elapsedSinceLastShot[i] = 0;
            elapsedBetweenEachShot[i] = 1 / fireRate[i];
        }
            camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        fireAudio = GameObject.Find("SampleFire").GetComponent<AudioSource>();
        projectileManager = GameObject.Find("Projectile Manager");
        bulletEmitter = GameObject.Find("Bullet emitter");
        playerOwner = transform.parent.transform.parent.Find("Player Entity").gameObject;
    }

    // Update is called once per frame
    public void Update()
    {
        for (int i = 0; i != elapsedSinceLastShot.Length; i++)
        {
            elapsedSinceLastShot[i] += Time.deltaTime;
        }

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

        if (!Input.GetButton("Fire1"))
        {                  
            Fire1Up();   
        }                  
                           
        if (!Input.GetButton("Fire2"))
        {
            Fire2Up();
        }

        if (!isLeftClickDown && Input.GetButton("Fire1"))
        {
            isLeftClickDown = true;
        }
        else if (isLeftClickDown && !Input.GetButton("Fire1"))
        {
            Fire1UpOnce();
            isLeftClickDown = false;
        }


        if (!isRightClickDown && Input.GetButton("Fire2"))
        {
            isRightClickDown = true;
        }
        else if (isRightClickDown && !Input.GetButton("Fire2"))
        {
            Fire2UpOnce();
            isRightClickDown = false;
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

    protected virtual void Fire1UpOnce()
    {

    }
    protected virtual void Fire2UpOnce()
    {

    }

    protected virtual void Fire1Up()
    {

    }
    protected virtual void Fire2Up()
    {

    }

    protected virtual void Fire2Once()
    {

    }

    // Returns true and resets elapsedSinceLastShot if can be fired. Parameter chooses for which weapon fire
    protected bool CheckCanFire(int i)
    {
        i -= 1;
        if (elapsedSinceLastShot[i] >= elapsedBetweenEachShot[i])
        {
            elapsedSinceLastShot[i] = 0;
            return true;
        }
        return false;
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
