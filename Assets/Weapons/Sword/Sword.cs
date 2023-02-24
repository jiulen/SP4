using Unity.Burst.CompilerServices;
using UnityEngine;

public class Sword : WeaponBase
{
    // Start is called before the first frame update
    private FPS player;
    private float minStrength, currentStrength;
    public GameObject KnifeProjectile;
    private Vector3 storeOGPosition;
    private Quaternion storeOGRotation;

    private float KnifeThrowCooldown = 2.0f, elaspe = 0;
    public Animator GetAnimator()
    {
        return animator;
    }

    void Start()
    {
        base.Start();
        player = owner.GetComponent<FPS>();
        animator = transform.Find("Gun/sword").GetComponent<Animator>();
        storeOGPosition = transform.Find("Gun/sword").localPosition;
        storeOGRotation = transform.Find("Gun/sword").localRotation;
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

            if ((AnimatorIsPlaying("Throw") && !animator.GetBool("TrThrow"))) {
                //if (currentStrength > minStrength)
                    ThrowKnife();
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
            {
                transform.Find("Gun/sword").localPosition = storeOGPosition;
                transform.Find("Gun/sword").localRotation = storeOGRotation;
            }
        }

        if (!weaponModel.activeSelf)
        {
            if (elaspe >= KnifeThrowCooldown)
            {
                weaponModel.SetActive(true);
                elaspe = 0;
            }
            elaspe += Time.deltaTime;
        }
    }

    protected override void Fire2()
    {
        if (!animator.GetBool("TrSlice") && weaponModel.activeSelf)
        {
            animator.SetBool("TrHold", true);
            animator.SetBool("TrThrow", false);
            animator.SetBool("TrSlice", false);
        }
    }

    protected override void Fire1Once()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Empty") && weaponModel.activeSelf)
        {
            animator.SetBool("TrSlice", true);
            animator.SetBool("TrThrow", false);
            animator.SetBool("TrHold", false);
            player.SetForcedash(true);
        }
    }

    public void OnChildTriggerEnter(Collider other, int damage)
    {
        if (other.tag == "PlayerHitBox")
        {
            EntityBase player = other.GetComponent<PlayerHitBox>().owner.GetComponent<EntityBase>();
            if (player != null)
            {
                if (player.gameObject != owner)
                {
                    Vector3 dir = owner.transform.position - player.transform.position;
                    if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit))
                    {
                        if (other.name == "Head")
                        {
                            particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal, 15);
                            player.TakeDamage(damage * 2, dir, owner, this.gameObject);
                        }
                        else
                        {
                            particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal);
                            player.TakeDamage(damage, dir, owner, this.gameObject);
                        }
                    }
                }
            }
        }
        else
        {
            EntityBase entity = other.GetComponent<EntityBase>();
            if (entity != null)
            {
                if (entity.gameObject != owner)
                {
                    Vector3 dir = owner.transform.position - entity.transform.position;
                    entity.TakeDamage(damage, dir, owner, this.gameObject);
                }
            }
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit))
                particleManager.GetComponent<ParticleManager>().CreateEffect("Sparks_PE", hit.point, hit.normal);
        }
    }

    protected override void Fire2UpOnce()
    {
        animator.SetBool("TrThrow", true);
        animator.SetBool("TrHold", false);
        animator.SetBool("TrSlice", false);
    }


    void ThrowKnife()
    {
        Transform newTransform = camera.transform;
        Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
        GameObject go = Instantiate(KnifeProjectile, bulletEmitter.transform);
        go.GetComponent<ProjectileBase>().SetWeaponUsed(this.gameObject);
        go.GetComponent<ProjectileBase>().SetObjectReferences(owner, particleManager);
        go.GetComponent<SwordProjectile>().damage = damage[0];
        go.GetComponent<Rigidbody>().velocity = front.normalized * projectileVel[0];
        go.transform.SetParent(projectileManager.transform);
        currentStrength = 0;
        weaponModel.SetActive(false);
        //fireAudio.Play();
    }
}
