using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Sniper : WeaponBase
{
    public GameObject SCATBulletPF;
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
    FPS player;

    private void Awake()
    {
        storeOGPosition = transform.Find("Gun/AWP").localPosition;
        storeOGRotation = transform.Find("Gun/AWP").localRotation;
    }

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

    private void LateUpdate()
    {
        if (Scoped.enabled)
            camera.transform.position += camera.transform.up * (Mathf.Sin(stablizeElasped * 2) / 2) * 0.4f + camera.transform.right * Mathf.Cos(stablizeElasped) * 0.4f;
    }

    protected override void Fire2Up()
    {
        Scoped.enabled = false;
        PlayOnce = false;
        camaim.ZoomOut();
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
                if (Physics.Raycast(bulletEmitter.transform.position, front, out RaycastHit hit))
                {
                    EntityBase entity = hit.transform.gameObject.GetComponent<EntityBase>();
                    if (entity != null)
                    {
                        Vector3 dir = -front;
                        entity.TakeDamage(damage[0], dir);
                    }
                }

                if (hit.collider.tag == "PlayerHitBox")
                {
                    if (hit.collider.name == "Head")
                    {
                        particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal, 15);
                    }
                    else
                    {
                        particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal);

                    }
                }
                else
                    particleManager.GetComponent<ParticleManager>().CreateEffect("Sparks_PE", hit.point, hit.normal);

                animator.SetBool("isActive", true);
                fireAudio.Play();

            }
        }
    }

    override protected void Fire2() // right click press and hold
    {
        if (animationdone)
        {
            Scoped.enabled = true;
            if (!PlayOnce)
            {
                ScopeSound.Play();
                PlayOnce = true;
            }
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
}
