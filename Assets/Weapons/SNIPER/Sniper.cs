using System.Collections;
using System.Collections.Generic;
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
    private GameObject SniperModel;
    private Vector3 temp;
    private float DmgReduction;
    public float stablizeElasped = 0;
    float StablizeDuration = 10f, StablizeProgress, AnimationRate = 0.1f;
    private bool PlayOnce = false;
    private Text DmgReductionTxt;

    //public ParticleSystem hiteffect;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        slider = GetComponentInChildren<Slider>();
        Scoped = transform.Find("SniperUICanvas").GetComponent<Canvas>();
        camaim = GetComponent<AimZoom>();
        slider = GetComponentInChildren<Slider>();
        ScopeSound = GameObject.Find("ScopeSound").GetComponent<AudioSource>();
        StablizeProgress = StablizeDuration;
        slider.maxValue = slider.value = StablizeProgress;
        SniperModel = GameObject.Find("AWP");
        DmgReductionTxt = GameObject.Find("DamageReductionTxt").GetComponent<Text>();
    }

    //Update is called once per frame
    void Update()
    {
        FPS player = transform.root.GetComponentInChildren<FPS>();

        base.Update();

        if (!Input.GetButton("Fire2")) // scope
        {
            Scoped.enabled = false;
            PlayOnce = false;
            camaim.ZoomOut();
            desiredPositionAnimation = new Vector3(0.25f, -0.2f, 0.75f);
            player.candash = true;
            SniperModel.SetActive(true);
        }
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, desiredPositionAnimation, ref velocity, AnimationRate);

        if ((transform.localPosition - temp).magnitude <= 0.01f)
            animationdone = true;
        else
            animationdone = false;

        UpdateStablize();

    }

    override protected void Fire1() // right click press and hold
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
                    entity.TakeDamage(1);
                }
            }
            fireAudio.Play();
        }
    }    

    override protected void Fire2() // right click press and hold
    {
        FPS player = transform.root.GetComponentInChildren<FPS>();

        if (animationdone)
        {
            Scoped.enabled = true; // change later
            if (!PlayOnce)
            {
                ScopeSound.Play();
                PlayOnce = true;
            }
            camaim.ZoomIn();
            SniperModel.SetActive(false);
        }
        temp = desiredPositionAnimation = new Vector3(0.0f, -0.15f, 0.58f);
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
