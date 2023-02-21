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
    FPS player;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        player = owner.GetComponent<FPS>();
        slider = GetComponentInChildren<Slider>();
        slider.maxValue = PowerMaxScale;
        slider.minValue = slider.value = PowerCurrentScale;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

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
        if (CheckCanFire(1))
        {
            Transform newTransform = camera.transform;
            Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
            GameObject go = Instantiate(Rocket, bulletEmitter.transform);
            go.GetComponent<Rocket>().damage = damage[0];
            go.GetComponent<Rocket>().SetCreator(owner);
            go.GetComponent<Rigidbody>().velocity = front.normalized * projectileVel[1] * PowerCurrentScale;
            go.transform.SetParent(projectileManager.transform);
            PowerCurrentScale = 1;
            fireAudio.Play();
        }
    }

    private void UpdateSlider()
    {
        slider.value = PowerCurrentScale;
    }
}
