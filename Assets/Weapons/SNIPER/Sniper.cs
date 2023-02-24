using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Sniper : WeaponBase
{
    private Vector3 front;
    public Canvas Scoped;
    private Slider slider;
    private AimZoom camaim;
    private Vector3 velocity = Vector3.zero;
    private Vector3 desiredPositionAnimation;
    private bool animationdone = true;
    private AudioSource ScopeSound;
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
        ScopeSound = GameObject.Find("ScopeSound").GetComponent<AudioSource>();
        StablizeProgress = StablizeDuration;
        slider.maxValue = slider.value = StablizeProgress;
        DmgReductionTxt = transform.Find("SniperUICanvas").GetComponentInChildren<Text>();
        animator = transform.Find("Gun/AWP").GetComponent<Animator>();
        storeOGPosition = transform.Find("Gun/AWP").localPosition;
        storeOGRotation = transform.Find("Gun/AWP").localRotation;
    }

    //Update is called once per frame
    void Update()
    {
        base.Update();

        if ((AnimatorIsPlaying("SniperRecoil") && animator.GetBool("isActive")))
        {
            animator.SetBool("isActive", false);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
        {
            if (transform.Find("Gun/AWP") != null)
            {
                transform.Find("Gun/AWP").localPosition = storeOGPosition;
                transform.Find("Gun/AWP").localRotation = storeOGRotation;
            }
        }
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, desiredPositionAnimation, ref velocity, AnimationRate);

        if ((transform.localPosition - ScopeDesiredPosition).magnitude <= 0.01f)
            animationdone = true;
        else
            animationdone = false;

        UpdateStablize();

    }

    private void LateUpdate()
    {
        if (Scoped.enabled)
            camera.transform.position += camera.transform.up * (Mathf.Sin(stablizeElasped * 2) / 2) * 0.4f + camera.transform.right * Mathf.Cos(stablizeElasped) * 0.4f;
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
                Transform newTransform = camera.transform;
                front = newTransform.forward * 1000 - bulletEmitter.transform.position;

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

                                    //Do bullet tracer (if hit)
                                    trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);
                                    //Spawn bullet tracer
                                    StartCoroutine(SpawnTrail(trail, hit2.point, hit2.normal, hit2.collider, true));

                                    if (hit2.collider.name == "Head")
                                    {
                                        particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit2.point, hit2.normal, 15);
                                        player2.TakeDamage(damage[0] * 2, -front);
                                    }
                                    else
                                    {
                                        particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit2.point, hit2.normal);
                                        player2.TakeDamage(damage[0] * 2, -front);
                                    }
                                }
                                else
                                {
                                    //Do bullet tracer (if hit)
                                    trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);
                                    //Spawn bullet tracer
                                    StartCoroutine(SpawnTrail(trail, hit2.point, hit2.normal, hit2.collider, true));

                                    particleManager.GetComponent<ParticleManager>().CreateEffect("Sparks_PE", hit2.point, hit2.normal);

                                    EntityBase entity2 = hit2.transform.gameObject.GetComponent<EntityBase>();
                                    if (entity2 != null)
                                    {
                                        entity2.TakeDamage(damage[0], -front);
                                    }
                                }
                            }
                            else
                            {
                                //Do bullet tracer (if no hit)
                                trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);

                                //Spawn bullet tracer
                                StartCoroutine(SpawnTrail(trail, bulletEmitter.transform.position + front * 200, Vector3.zero, null, false));
                            }
                        }
                        else
                        {
                            //Do bullet tracer (if hit)
                            trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);
                            //Spawn bullet tracer
                            StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, hit.collider, true));

                            if (hit.collider.name == "Head")
                            {
                                particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal, 15);
                                player.TakeDamage(damage[0] * 2, -front);
                            }
                            else
                            {
                                particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal);
                                player.TakeDamage(damage[0] * 2, -front);
                            }
                        }

                        //Do bullet tracer (if hit)
                        trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);
                        //Spawn bullet tracer
                        StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, hit.collider, true));

                        if (hit.collider.name == "Head")
                        {
                            particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal, 15);
                            player.TakeDamage(damage[0] * 2, -front, owner, this.gameObject);
                        }
                        else
                        {
                            particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal);
                            player.TakeDamage(damage[0], -front, owner, this.gameObject);

                        }
                    }
                    else
                    {
                        //Do bullet tracer (if hit)
                        trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);
                        //Spawn bullet tracer
                        StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, hit.collider, true));

                        particleManager.GetComponent<ParticleManager>().CreateEffect("Sparks_PE", hit.point, hit.normal);

                        EntityBase entity = hit.transform.gameObject.GetComponent<EntityBase>();
                        if (entity != null)
                        {
                            entity.TakeDamage(damage[0], -front, owner, this.gameObject);
                        }
                    }
                }
                else
                {
                    //Do bullet tracer (if no hit)
                    trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);

                    //Spawn bullet tracer
                    StartCoroutine(SpawnTrail(trail, bulletEmitter.transform.position + front * 200, Vector3.zero, null, false));
                }
                animator.SetBool("isActive", true);
                fireAudio.Play();

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
                ScopeSound.Play();
                PlayOnce = true;
            }
            //if (!player.candash)
                //camaim.ZoomIn();
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


    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal, Collider hitCollider, bool madeImpact)
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

        if (madeImpact && hitCollider)
        {
            //do hit effects here
        }

        Destroy(trail.gameObject, trail.time);
    }
}
