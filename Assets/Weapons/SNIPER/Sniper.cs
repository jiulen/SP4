using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class Sniper : WeaponBase
{
    public GameObject SCATBulletPF;
    GameObject projectileManager;
    GameObject bulletEmitter;
    GameObject bulletEmitterFront;
    private Vector3 front;
    public Canvas Scoped;
    private Slider slider;
    private AimZoom camaim;
    private Vector3 velocity = Vector3.zero;
    private Vector3 desiredPositionAnimation;
    private bool animationdone = true;
    private AudioSource fireAudio, ScopeSound;
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
        bulletEmitter = GameObject.Find("Bullet emitter");
        projectileManager = GameObject.Find("Projectile Manager");
        bulletEmitterFront = GameObject.Find("Bullet emitter front");
        camaim = GetComponent<AimZoom>();
        slider = GetComponentInChildren<Slider>();
        fireAudio = GameObject.Find("SampleFire").GetComponent<AudioSource>();
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

        if (player != null)
        {

            if (Input.GetButton("Fire2")) // scope
            {
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
            else
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

            if (Input.GetButton("Fire1"))
            {
                elapsedSinceLastShot += Time.deltaTime;
                if (elapsedSinceLastShot >= elapsedBetweenEachShot)
                {
                    // for now
                    //GameObject bullet = Instantiate(SCATBulletPF, bulletEmitter.transform);
                    front = bulletEmitterFront.transform.position - bulletEmitter.transform.position;
                    //bullet.GetComponent<Rigidbody>().velocity = front * projectileVel;
                    //bullet.transform.SetParent(projectileManager.transform);

                    // new
                    if (Physics.Raycast(bulletEmitter.transform.position, front, out RaycastHit hit))
                    {
                        //Instantiate(hiteffect, hit.point, Quaternion.LookRotation(hit.normal));
                        //Destroy(hiteffect);

                        if (hit.transform.tag == "Crates")
                        {
                            DestructibleObjects destructibleObjects = hit.transform.GetComponent<DestructibleObjects>();
                            destructibleObjects.DestroyDestructible();
                        }
                    }




                    elapsedSinceLastShot = 0;
                    fireAudio.Play();
                }
            }
        }
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
