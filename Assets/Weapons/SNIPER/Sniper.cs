using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Sniper : WeaponBase
{
    private Vector3 front;
    public Canvas Scoped;
    private Slider slider;
    private AimZoom camaim;
    private Vector3 velocity = Vector3.zero;
    private Vector3 desiredPositionAnimation;
    private bool animationdone = true;
    private float DmgReduction;
    public float stablizeElasped = 0;
    float StablizeDuration = 10f, StablizeProgress, AnimationRate = 0.1f;
    private bool PlayOnce = false;
    private Text DmgReductionTxt;
    private Vector3 ScopeDesiredPosition;
    private Vector3 storeOGPosition;
    private Quaternion storeOGRotation;
    [SerializeField] TrailRenderer bulletTrail;
    const float trailSpeed = 200f;
    FPS player;

    [Header("Audio References")]
    public AudioSource AudioFire1;
    public AudioSource AudioScopeSound;

    //private void Awake()
    //{
    //    storeOGPosition = transform.Find("Gun/AWP").localPosition;
    //    storeOGRotation = transform.Find("Gun/AWP").localRotation;
    //}

    //public ParticleSystem hiteffect;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        player = owner.GetComponent<FPS>();
        slider = GetComponentInChildren<Slider>();
        Scoped = transform.Find("SniperUICanvas").GetComponent<Canvas>();
        camaim = GetComponent<AimZoom>();
        slider = GetComponentInChildren<Slider>();
        StablizeProgress = StablizeDuration;
        slider.maxValue = slider.value = StablizeProgress;
        DmgReductionTxt = transform.Find("SniperUICanvas").GetComponentInChildren<Text>();
        animator = transform.Find("Gun/AWP").GetComponent<Animator>();
        storeOGPosition = transform.Find("Gun/AWP").localPosition;
        storeOGRotation = transform.Find("Gun/AWP").localRotation;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            transform.Find("SniperUICanvas").gameObject.SetActive(true);
        }
    }

    //Update is called once per frame
    void Update()
    {
        base.Update();

        if (IsOwner)
        {
            if ((AnimatorIsPlaying("SniperRecoil") && animator.GetBool("isActive")))
            {
                animator.SetBool("isActive", false);
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
            {
                transform.Find("Gun/AWP").localPosition = storeOGPosition;
                transform.Find("Gun/AWP").localRotation = storeOGRotation;
            }
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, desiredPositionAnimation, ref velocity, AnimationRate);

            if ((transform.localPosition - ScopeDesiredPosition).magnitude <= 0.01f)
                animationdone = true;
            else
                animationdone = false;

            UpdateStablize();
        }
    }

    private void LateUpdate()
    {
        if (IsOwner)
        {
            if (Scoped.enabled)
                camera.transform.position += camera.transform.up * (Mathf.Sin(stablizeElasped * 2) / 2) * 0.4f + camera.transform.right * Mathf.Cos(stablizeElasped) * 0.4f;
        }
    }

    protected override void Fire2Up()
    {
        Scoped.enabled = false;
        PlayOnce = false;
        //camaim.ZoomOut();
        desiredPositionAnimation = new Vector3(0.25f, -0.2f, 0.75f);
        player.candash = true;
        weaponModel.SetActive(true);
    }

    override protected void Fire1Once()
    {
        if (!animator.GetBool("isActive"))
        {
            if (CheckCanFire(1))
            {
                AudioFire1.Play();

                Transform newTransform = camera.transform;
                int hitType = -1;
                front = newTransform.forward * 1000 - bulletEmitter.transform.position;

                if (Scoped.enabled)
                    front = newTransform.forward * 1000;
                // new
                TrailRenderer trail = null;
                if (Physics.Raycast(bulletEmitter.transform.position, front, out RaycastHit hit))
                {
                    if (hit.collider.tag == "PlayerHitBox")
                    {
                        EntityBase player = hit.collider.gameObject.GetComponent<PlayerHitBox>().owner.GetComponent<EntityBase>();

                        if (player.gameObject == owner)
                        {
                            //Do second hitscan
                            RaycastHit hit2;
                            if (Physics.Raycast(hit.point + front * 0.1f, front, out hit2))
                            {
                                if (hit2.collider.tag == "PlayerHitBox")
                                {
                                    EntityBase player2 = hit2.collider.gameObject.GetComponent<PlayerHitBox>().owner.GetComponent<EntityBase>();
                                    //Shouldnt need to check for own player again

                                    if (hit2.collider.name == "Head")
                                    {
                                        hitType = 2;
                                        player2.TakeDamage(damage[0] * 2, front, owner, gameObject);
                                    }
                                    else
                                    {
                                        hitType = 3;
                                        player2.TakeDamage(damage[0] * 2, front, owner, gameObject);
                                    }

                                    CreateTrailServerRpc(hit2.point, hit2.normal, hitType);
                                }
                                else
                                {
                                    hitType = 1;

                                    EntityBase entity2 = hit2.transform.gameObject.GetComponent<EntityBase>();
                                    if (entity2 != null)
                                    {
                                        entity2.TakeDamage(damage[0], front, owner, gameObject);
                                    }

                                    CreateTrailServerRpc(hit2.point, hit2.normal, hitType);
                                }
                            }
                            else
                            {
                                hitType = 0;

                                CreateTrailServerRpc(bulletEmitter.transform.position + front * 200, Vector3.zero, hitType);
                            }
                        }
                        else
                        {
                            if (hit.collider.name == "Head")
                            {
                                hitType = 2;
                                player.TakeDamage(damage[0] * 2, front, owner, gameObject);
                            }
                            else
                            {
                                hitType = 3;
                                player.TakeDamage(damage[0] * 2, front, owner, gameObject);
                            }

                            CreateTrailServerRpc(hit.point, hit.normal, hitType);
                        }
                    }
                    else
                    {
                        hitType = 1;

                        EntityBase entity = hit.transform.gameObject.GetComponent<EntityBase>();
                        if (entity != null)
                        {
                            entity.TakeDamage(damage[0], front, owner, this.gameObject);
                        }

                        CreateTrailServerRpc(hit.point, hit.normal, hitType);
                    }
                }
                else
                {
                    hitType = 0; 
                    
                    CreateTrailServerRpc(bulletEmitter.transform.position + front * 200, Vector3.zero, hitType);
                }
                animator.SetBool("isActive", true);
                AudioFire1.Play();

            }
        }
    }

    override protected void Fire2()
    {
        if (animationdone)
        {
            Scoped.enabled = true;
            if (!PlayOnce)
            {
                AudioScopeSound.Play();
                PlayOnce = true;
            }
            if (!player.candash)
                camaim.ZoomIn();
            weaponModel.SetActive(false);
        }
        ScopeDesiredPosition = desiredPositionAnimation = new Vector3(0.0f, -0.15f, 0.58f);
        player.candash = false;
    }

    void UpdateStablize()
    {
        if (Scoped.enabled)
        {
            stablizeElasped += Time.deltaTime;
            StablizeProgress -= Time.deltaTime;
        }
        else
        {
            stablizeElasped = 0;
            StablizeProgress = StablizeDuration;
        }

        if (StablizeProgress <= 0)
            StablizeProgress = 0;

        UpdateSlider(StablizeProgress);

        DmgReduction = (StablizeProgress / StablizeDuration);
        if (DmgReduction <= .1f)
            DmgReduction = .1f;
        DmgReductionTxt.text = (int)(DmgReduction * 100f) + "%";
    }

    void UpdateSlider(float value)
    {
        slider.value = value;
    }


    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint)
    {
        Vector3 startPos = trail.transform.position;

        float distance = Vector3.Distance(trail.transform.position, hitPoint);
        float startDistance = distance;

        while (distance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPos, hitPoint, 1 - (distance / startDistance));
            distance -= Time.deltaTime * trailSpeed;

            yield return null;
        }

        trail.transform.position = hitPoint;

        Destroy(trail.gameObject, trail.time);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateTrailServerRpc(Vector3 hitPoint, Vector3 hitNormal, int hitType) //Calls the ClientRpc function
    {
        CreateTrailClientRpc(hitPoint, hitNormal, hitType);
    }

    [ClientRpc]
    private void CreateTrailClientRpc(Vector3 hitPoint, Vector3 hitNormal, int hitType) //hitType: 0 - no hit, 1 - hit terrain, 2 - hit player head, 3 - hit player body
    {
        //Make trail renderer
        TrailRenderer trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);

        //Spawn bullet tracer
        StartCoroutine(SpawnTrail(trail, hitPoint));

        //Hit particle effects
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
}
