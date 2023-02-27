using Unity.Burst.CompilerServices;
using UnityEngine;
using Unity.Netcode;

public class Sword : WeaponBase
{
    // Start is called before the first frame update
    private FPS player;
    private float minStrength, currentStrength;
    public GameObject KnifeProjectile;
    private Vector3 storeOGPosition;
    private Quaternion storeOGRotation;

    [SerializeField] MeshCollider swordModelCollider;

    private float KnifeThrowCooldown = 2.0f, elaspe = 0;

    public AudioSource AudioThrow;

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

        if (IsOwner)
        {
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

                if (AnimatorIsPlaying("Throw") && !animator.GetBool("TrThrow") && weaponModel.activeSelf)
                {
                    //if (currentStrength > minStrength)

                    Transform newTransform = camera.transform;
                    Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
                    PlayAudioServerRpc();

                    ThrowKnifeServerRpc(front);
                    weaponModel.SetActive(false);
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
                    SetKnifeModelActiveServerRpc(true);
                    elaspe = 0;
                }
                elaspe += Time.deltaTime;
            }
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
                            SpawnHitParticleServerRpc(hit.point, hit.normal, 2);
                            player.TakeDamage(damage * 2, dir, owner, gameObject);
                        }
                        else
                        {
                            SpawnHitParticleServerRpc(hit.point, hit.normal, 3);
                            player.TakeDamage(damage, dir, owner, gameObject);
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
                SpawnHitParticleServerRpc(hit.point, hit.normal, 1);
        }
    }

    protected override void Fire2UpOnce()
    {
        animator.SetBool("TrThrow", true);
        animator.SetBool("TrHold", false);
        animator.SetBool("TrSlice", false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ThrowKnifeServerRpc(Vector3 front)
    {
        GameObject go = Instantiate(KnifeProjectile, bulletEmitter.transform);
        go.GetComponent<NetworkObject>().Spawn();
        go.GetComponent<NetworkObject>().TrySetParent(projectileManager);
        go.GetComponent<ProjectileBase>().SetWeaponUsed(gameObject);
        go.GetComponent<SwordProjectile>().damage = damage[0];
        go.GetComponent<ProjectileBase>().SetObjectReferencesClientRpc(owner.GetComponent<NetworkObject>().NetworkObjectId,
                                                                       particleManager.GetComponent<NetworkObject>().NetworkObjectId);
        go.GetComponent<SwordProjectile>().SetVelocity(front.normalized * projectileVel[0]);
        currentStrength = 0;
        SetKnifeModelActiveClientRpc(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKnifeModelActiveServerRpc(bool active)
    {
        SetKnifeModelActiveClientRpc(active);
    }

    [ClientRpc]
    private void SetKnifeModelActiveClientRpc(bool active)
    {
        weaponModel.SetActive(active);
        swordModelCollider.enabled = active;
    }

    [ServerRpc]
    private void SpawnHitParticleServerRpc(Vector3 hitPoint, Vector3 hitNormal, int hitType)
    {
        SpawnHitParticleClientRpc(hitPoint, hitNormal, hitType);
    }

    [ClientRpc]
    private void SpawnHitParticleClientRpc(Vector3 hitPoint, Vector3 hitNormal, int hitType)
    {
        switch (hitType)
        {
            case 1:
                particleManager.GetComponent<ParticleManager>().CreateEffect("Sparks_PE", hitPoint, hitNormal);
                break;
            case 2:
                particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hitPoint, hitNormal, 15);
                break;
            case 3:
                particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hitPoint, hitNormal);
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayAudioServerRpc()
    {
        PlayAudioClientRpc();
    }

    [ClientRpc]
    void PlayAudioClientRpc()
    {
        AudioThrow.Play();
    }
}
