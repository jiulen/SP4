using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG : WeaponBase
{
    private Vector3 desiredPositionAnimation, velocity;
    float AnimationRate = 0.1f;
    public GameObject Rocket;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        FPS player = transform.root.GetComponentInChildren<FPS>();
        base.Update();

        if (!Input.GetButton("Fire2"))
        {
            desiredPositionAnimation = new Vector3(0.22f, -0.2f, 0.3f);
            player.candash = true;
        }
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, desiredPositionAnimation, ref velocity, AnimationRate);

    }

    protected override void Fire2()
    {
        FPS player = transform.root.GetComponentInChildren<FPS>();

        desiredPositionAnimation = new Vector3(0.078f, -0.2f, 0.35f);
        player.candash = false;
    }

    override protected void Fire1Once()
    {
        if (CheckCanFire(1))
        {
            Transform newTransform = camera.transform;
            Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
            GameObject go = Instantiate(Rocket, bulletEmitter.transform);
            go.GetComponent<Rigidbody>().velocity = front.normalized * projectileVel[1];
            Rocket.GetComponent<Rocket>().SetCreator(playerOwner);
            go.transform.SetParent(projectileManager.transform);
        }
    }
}
