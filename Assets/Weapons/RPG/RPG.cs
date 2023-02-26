using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

    [SerializeField] GameObject ChargeUI;

    [Header("Audio References")]
    public AudioSource AudioFire1;
    public AudioSource AudioFire2;

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

        storeOGPosition = transform.Find("Gun/RPG7").localPosition;
        storeOGRotation = transform.Find("Gun/RPG7").localRotation;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            ChargeUI.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (IsOwner)
        {
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
        if (!animator.GetBool("isActive"))
        {
            PowerCurrentScale += Time.deltaTime;
            slider.enabled = true;
        }
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
                ShootRocketServerRpc(front, bulletEmitter.transform.position);
                AudioFire1.Play();
                PowerCurrentScale = 1;
                RocketMuzzle.SetActive(false);
                animator.SetBool("isActive", true);
            }
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void ShootRocketServerRpc(Vector3 front, Vector3 spawnposition)
    {
        GameObject go = Instantiate(Rocket, spawnposition, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
        go.GetComponent<NetworkObject>().TrySetParent(projectileManager);
        go.GetComponent<ProjectileBase>().SetWeaponUsed(this.gameObject);
        go.GetComponent<Rocket>().damage = damage[0];
        go.GetComponent<Rocket>().SetObjectReferencesClientRpc(owner.GetComponent<NetworkObject>().NetworkObjectId, 
                                                               particleManager.GetComponent<NetworkObject>().NetworkObjectId);
        go.GetComponent<Rocket>().SetVelocity(front.normalized * projectileVel[0] * PowerCurrentScale);
    }

    private void UpdateSlider()
    {
        slider.value = PowerCurrentScale;
    }
}
