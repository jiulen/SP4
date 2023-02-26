using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.Netcode;
public class WeaponBase : NetworkBehaviour
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

    protected float bloomProgress = 0;
    protected float bloomMax = 100;

    protected Camera camera;
    protected GameObject projectileManager;
    protected GameObject particleManager;
    protected GameObject bulletEmitter;
    protected GameObject weaponModel;
    protected Vector3 saveStartingWeaponPosition;
    public Sprite WeaponIcon;
    public GameObject owner;
    protected PlayerEntity ownerPlayerEntityScript;
    private bool isLeftClickDown = false, isRightClickDown = false;
    protected Animator animator;
    protected ParticleSystem muzzleFlash;
    protected Animator fireAnimation;
    protected CustomCrosshair crosshair;
    private GameObject wheelManagerUI;

    public GameObject GetOwner()
    {
        return owner;
    }
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
        projectileManager = GameObject.Find("Projectile Manager");
        particleManager = GameObject.Find("Particle Manager");
        bulletEmitter = transform.Find("Gun/Bullet emitter").gameObject;
        //playerOwner = transform.parent.transform.parent.Find("Player Entity").gameObject;
        owner = transform.parent.parent.parent.gameObject;   // this > right hand > equipped > player
        ownerPlayerEntityScript = owner.GetComponent<PlayerEntity>();
        weaponModel = transform.Find("Gun").gameObject;
        saveStartingWeaponPosition = weaponModel.transform.localPosition;
        if (bulletEmitter.transform.childCount > 0)
            muzzleFlash = bulletEmitter.transform.GetChild(0).GetComponent<ParticleSystem>();

        crosshair = ownerPlayerEntityScript.GetCrosshairCanvas().GetComponent<CustomCrosshair>();
        wheelManagerUI = ownerPlayerEntityScript.GetWeaponWheelCanvas().gameObject;
        Debug.LogWarning(wheelManagerUI);
    }

    // Update is called once per frame
    public void Update()
    {
        if (IsOwner)
        {
            for (int i = 0; i != elapsedSinceLastShot.Length; i++)
            {
                elapsedSinceLastShot[i] += Time.deltaTime;
            }

            if (!wheelManagerUI.transform.GetChild(0).gameObject.activeSelf)
            {
                if (Input.GetButton("Fire1"))
                {
                    Fire1();
                }
                else
                    Fire1Up();

                if (Input.GetButton("Fire2"))
                {
                    Fire2();
                }
                else
                    Fire2Up();

                if (Input.GetButtonDown("Fire1"))
                {
                    Fire1Once();
                }

                if (Input.GetButtonDown("Fire2"))
                {
                    Fire2Once();
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

            crosshair.UpdateBloom(bloomProgress, bloomMax);
        }
    }

    protected virtual void Fire1()
    {

    }

    protected virtual void Fire2()
    {

    }
    protected virtual void Fire1Once() { }
    protected virtual void Fire1UpOnce() { }
    protected virtual void Fire2UpOnce() { }
    protected virtual void Fire1Up() { }
    protected virtual void Fire2Up() { }
    protected virtual void Fire2Once() { }


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

    // Checks if a hitscan collision with a hitbox that belongs to a player, and returns true if player is same as the weapon that did the hitscan
    //public bool CheckIfCreator(GameObject other)
    //{
    //    Debug.LogError("HIT" + other.transform.name);

    //    if (other.tag == "PlayerHitBox")
    //    {
    //        GameObject hitBoxOwner = other.GetComponent<PlayerHitBox>().owner;

    //        if (hitBoxOwner == owner)
    //            return true;
    //        return false;
    //    }
    //    return false;
    //}


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
    public static Vector3 RandomSpray(Vector3 front, float maxInnacuracy, float bloomProgress, float bloomMax)
    {
        float newMaxInaccuracy = maxInnacuracy * (bloomProgress / bloomMax);
        float randomPitch = Random.Range(-newMaxInaccuracy / 2, newMaxInaccuracy / 2);
        float randomYaw = Random.Range(-newMaxInaccuracy / 2, newMaxInaccuracy / 2);
        Quaternion pitchYawAdjustment = Quaternion.Euler(randomPitch, randomYaw, 0);
        Vector3 finalVector = pitchYawAdjustment * front;

        return finalVector.normalized;
    }

    public static Vector3 RandomSpray(Vector3 front, float maxInnacuracy)
    {
        float randomPitch = Random.Range(-maxInnacuracy / 2, maxInnacuracy / 2);
        float randomYaw = Random.Range(-maxInnacuracy / 2, maxInnacuracy / 2);
        Quaternion pitchYawAdjustment = Quaternion.Euler(randomPitch, randomYaw, 0);
        Vector3 finalVector = pitchYawAdjustment * front;

        return finalVector.normalized;
    }

    public bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f;
    }
}
