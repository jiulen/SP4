using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : WeaponBase
{
    // Start is called before the first frame update
    private FPS player;
    private float minStrength, currentStrength;
    public GameObject KnifeProjectile;
    private Vector3 storeOGPosition;
    private Quaternion storeOGRotation;

    public Animator GetAnimator()
    {
        return animator;
    }
    private void Awake()
    {
        storeOGPosition = transform.Find("Gun/sword").localPosition;
        storeOGRotation = transform.Find("Gun/sword").localRotation;
    }
    void Start()
    {
        base.Start();
        player = owner.GetComponent<FPS>();
        animator = transform.Find("Gun/sword").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (animator != null)
        {
            minStrength = animator.GetCurrentAnimatorStateInfo(0).length;
            currentStrength = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            if ((AnimatorIsPlaying("Throw") && animator.GetBool("TrThrow")) || (AnimatorIsPlaying("Slash") && animator.GetBool("TrSlice")))
            {
                animator.SetBool("TrHold", false);
                animator.SetBool("TrThrow", false);
                animator.SetBool("TrSlice", false);
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
            {
                transform.Find("Gun/sword").localPosition = storeOGPosition;
                transform.Find("Gun/sword").localRotation = storeOGRotation;
            }
        }
    }

    protected override void Fire2()
    {
        if (!animator.GetBool("TrSlice"))
        {
            animator.SetBool("TrHold", true);
            animator.SetBool("TrThrow", false);
            animator.SetBool("TrSlice", false);
        }
    }

    protected override void Fire1Once()
    {
        if (!animator.GetBool("TrHold"))
        {
            animator.SetBool("TrSlice", true);
            animator.SetBool("TrThrow", false);
            animator.SetBool("TrHold", false);
            player.SetForcedash(true);
        }
    }

    public void OnChildTriggerEnter(Collider other, int damage)
    {
        EntityBase entity = other.GetComponent<EntityBase>();
        if (entity != null)
        {
            if (entity.gameObject != owner)
            {
                Vector3 dir = owner.transform.position - entity.transform.position;
                entity.TakeDamage(damage, dir);
                Debug.Log("HIT");
            }
        }
    }

    protected override void Fire2UpOnce()
    {
        animator.SetBool("TrThrow", true);
        animator.SetBool("TrHold", false);
        animator.SetBool("TrSlice", false);
        if (currentStrength > minStrength)
            ThrowKnife();
    }


    void ThrowKnife()
    {
        Transform newTransform = camera.transform;
        Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
        GameObject go = Instantiate(KnifeProjectile, bulletEmitter.transform);
        go.GetComponent<ProjectileBase>().SetObjectReferences(owner, particleManager);
        go.GetComponent<SwordProjectile>().damage = damage[0];
        go.GetComponent<Rigidbody>().velocity = front.normalized * projectileVel[0];
        go.transform.SetParent(projectileManager.transform);
        //fireAudio.Play();
    }
}
