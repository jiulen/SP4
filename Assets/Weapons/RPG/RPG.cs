using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RPG : WeaponBase
{
    private Vector3 desiredPositionAnimation, velocity;
    private float PowerCurrentScale = 1, PowerMaxScale = 3f;
    private Slider slider;
    float AnimationRate = 0.1f;
    public GameObject Rocket;
    private GameObject RocketMuzzle;
    private Vector3 storeOGPosition;
    private Quaternion storeOGRotation;
    FPS player;
    private void Awake()
    {
        storeOGPosition = transform.Find("Gun/RPG7").localPosition;
        storeOGRotation = transform.Find("Gun/RPG7").localRotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        player = owner.GetComponent<FPS>();
        slider = GetComponentInChildren<Slider>();
        slider.maxValue = PowerMaxScale;
        slider.minValue = slider.value = PowerCurrentScale;
        animator = transform.Find("Gun/RPG7").GetComponent<Animator>();
        RocketMuzzle = transform.Find("Gun/RPG7/Rocket").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if ((AnimatorIsPlaying("RPGRecoil") && animator.GetBool("isActive")))
        {
            RocketMuzzle.SetActive(true);
            animator.SetBool("isActive", false);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
        {
            transform.Find("Gun/RPG7").localPosition = storeOGPosition;
            transform.Find("Gun/RPG7").localRotation = storeOGRotation;
        }

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, desiredPositionAnimation, ref velocity, AnimationRate);

        UpdateSlider();
    }

    protected override void Fire2Up()
    {
        desiredPositionAnimation = new Vector3(0.22f, -0.2f, 0.3f);
        player.candash = true;
    }

    protected override void Fire2()
    {
        desiredPositionAnimation = new Vector3(0.078f, -0.2f, 0.35f);
        player.candash = false;
    }

    override protected void Fire1()
    {
        PowerCurrentScale += Time.deltaTime;
        slider.enabled = true;
    }
    override protected void Fire1Up()
    {
        slider.enabled = false;
    }

    override protected void Fire1UpOnce()
    {
        if (!animator.GetBool("isActive"))
        {
            if (CheckCanFire(1))
            {
                Transform newTransform = camera.transform;
                Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
                GameObject go = Instantiate(Rocket, bulletEmitter.transform);
                go.GetComponent<Rocket>().damage = damage[0];
                go.GetComponent<Rocket>().SetObjectReferences(owner, particleManager);
                go.GetComponent<Rigidbody>().velocity = front.normalized * projectileVel[0] * PowerCurrentScale;
                go.transform.SetParent(projectileManager.transform);
                PowerCurrentScale = 1;
                fireAudio.Play();
                RocketMuzzle.SetActive(false);
                animator.SetBool("isActive", true);
            }
        }
    }

    private void UpdateSlider()
    {
        slider.value = PowerCurrentScale;
    }

}
