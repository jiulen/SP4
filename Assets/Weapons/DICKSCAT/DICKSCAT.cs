using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DICKSCAT : WeaponBase
{
    enum mode
    {
        DICK,
        SCAT
    }

    public float DICKDistance = 1;

    // Starting angle of 0 is Vec2(1, 0), starting angle of 45 is Vec2(0.5, 0.5)
    public float portalStartingAngle = 45;
    public int numPortals =4;

    private bool portalsActive = false;
    private float steamMax = 100;
    private float steamCurrent = 0;
    public float steamRate = 20;

    public GameObject SCATBulletPF;

    
    mode currentMode;
    GameObject portalParent;
    GameObject portal1;
    GameObject DICKFocalPoint;
    GameObject bulletEmitterFront;
    ParticleSystem muzzleFlashLight;

    public GameObject bloodEffect;
    public GameObject sparkEffect;

    Slider steamGauge;
    TMP_Text modeText;
   

    bool togglePortals = false;

    void Start()
    {
        base.Start();
        fireAnimation = weaponModel.transform.Find("M2").GetComponent<Animator>();
        fireAnimation.speed = 1/ (float)elapsedBetweenEachShot[1];

        portalParent = GameObject.Find("Portal Parent");
        portal1 = GameObject.Find("Portal Parent/Portal");
        DICKFocalPoint = GameObject.Find("DICK Focal Point");
        bulletEmitterFront = GameObject.Find("Bullet emitter front");

        steamGauge = GameObject.Find("UI canvas/Steam Gauge Slider").GetComponent<Slider>();
        modeText = GameObject.Find("UI canvas/Mode text").GetComponent<TMP_Text>();

        // Set camera for canvas
        transform.Find("UI canvas").GetComponent<Canvas>().worldCamera = camera;
        transform.Find("UI canvas").GetComponent<Canvas>().planeDistance = 0.5f;

        muzzleFlashLight = muzzleFlash.transform.Find("Light").GetComponent<ParticleSystem>();

        float arc = 180 + portalStartingAngle * 2;

        for (int i = 0; i != numPortals; i++)
        {
            float angle = portalStartingAngle + (arc / (numPortals - 1)) * i;
            float portalDistance = 1;
            Vector2 vec2 = MyMath.DegreeToVector2(angle);
            Vector3 vec3 = new Vector3(vec2.x, vec2.y, 0);
            GameObject portal;

            if(i == 0)
            {
                portal = portal1;
            }
            else
            {
                portal = Instantiate(portal1, portalParent.transform);
                portal.transform.parent = portalParent.transform;
                portal.transform.localPosition = new Vector3(0,0,0);
                //portal.transform.position = new Vector3(0, 0, 0);

                //portal.transform.position.Set(0, 0, 0);
                //Debug.Log(portal.transform.localPosition);
                //Debug.Log(i.ToString() + portal.transform.position);
            }

            portal.transform.Translate(vec3);
        }

        DICKFocalPoint.transform.Translate(0,0,DICKDistance);

        currentMode = mode.DICK;
    }
    private void TogglePortalsActive(bool active)
    {
        if(active != portalsActive)
        {
            Debug.Log("sed");
            portalsActive = active;
            foreach (Transform child in portalParent.transform)
            {
                child.gameObject.SetActive(active);
                if (active)
                {
                    ParticleSystem particleSystem = child.GetChild(1).GetComponent<ParticleSystem>();
                    particleSystem.Play();
                }
                //Transform laser = child.transform.GetChild(0);
                //laser.gameObject.SetActive(false);

            }
        }
       
    }

    void Update()
    {
        togglePortals = false;

        base.Update();

        steamGauge.value = steamCurrent;

        bloomProgress -= 100 * Time.deltaTime;
        if (bloomProgress < 0)
            bloomProgress = 0;

       
    }

    override protected void Fire1()
    {
        switch (currentMode)
        {
            case mode.DICK:
                {
                    togglePortals = true;
                    //portalParent.GetComponent<Transform>().eulerAngles = new Vector3(0, portalParent.GetComponent<Transform>().eulerAngles.y, currentRotate);

                    steamCurrent += steamRate * Time.deltaTime;
                    if (steamCurrent > steamMax)
                    {
                        steamCurrent = steamMax;
                    }


                    break;
                }
            case mode.SCAT:
                {
                    steamCurrent -= steamRate * Time.deltaTime;

                    if (steamCurrent < 0)
                    {
                        steamCurrent = 0;
                    }
                    if (CheckCanFire(2))
                    {
                        fireAnimation.enabled = true;

                        fireAnimation.Play("Fire Weapon");

                        muzzleFlash.GetComponent<ParticleSystem>().Stop();
                        muzzleFlash.GetComponent<ParticleSystem>().Play();

                        //muzzleFlashLight

                        Transform newTransform = camera.transform;
                        GameObject bullet = Instantiate(SCATBulletPF, bulletEmitter.transform);
                        bullet.GetComponent<ProjectileBase>().SetProjectileManager(projectileManager);
                        Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
                        bullet.GetComponent<Rigidbody>().velocity = RandomSpray(front.normalized, inaccuracy[1], bloomProgress, bloomMax) * projectileVel[1];
                        bullet.transform.SetParent(projectileManager.transform);
                        fireAudio.Play();

                        bloomProgress += 200 * (float)elapsedBetweenEachShot[1];
                        if (bloomProgress > bloomMax)
                            bloomProgress = bloomMax;
                    }
                    break;
                }
        }
    }

    override protected void Fire1Up()
    {
        if (CheckCanFire(2))
        {
            fireAnimation.enabled = false;
        }
    }

    override protected void Fire2()
    {
     
    }

    override protected void Fire1Once()
    {
        Debug.LogError("SEx");
        //fireAnimation.Play("Fire Weapon");
        fireAnimation.enabled = false;
    }

    override protected void Fire2Once()
    {
        switch (currentMode)
        {
            case mode.DICK:
                modeText.text = "S.C.A.T Mode";
                currentMode = mode.SCAT;
                break;

            case mode.SCAT:
                modeText.text = "D.I.C.K Mode";
                currentMode = mode.DICK;
                break;
        }
    }


    private void LateUpdate()
    {
        TogglePortalsActive(togglePortals);
        foreach (Transform child in portalParent.transform)
        {
            Vector3 midPos = (child.transform.position + DICKFocalPoint.transform.position) / 2;
            Transform laser = child.transform.GetChild(0);
            laser.gameObject.SetActive(true);
            laser.position = midPos;
            Vector3 direction = child.transform.position - DICKFocalPoint.transform.position;

            laser.rotation = Quaternion.LookRotation(direction);
            laser.Rotate(90, 0, 0);

            LineRenderer laserLine = laser.GetComponent<LineRenderer>();
            laserLine.positionCount = 2;
            laserLine.SetPosition(0, child.transform.position);

            //Debug.DrawRay(child.transform.position, 5 * (DICKFocalPoint.transform.position - child.transform.position), Color.red);
            Ray laserRayCast = new Ray(child.transform.position, 5 * (DICKFocalPoint.transform.position - child.transform.position));
            if (Physics.Raycast(laserRayCast, out RaycastHit hit, 50))
            {
                laserLine.SetPosition(1, child.transform.position + (DICKFocalPoint.transform.position - child.transform.position) * hit.distance);
                if (hit.collider.tag == "Player")
                {
                    Debug.Log("HIT!!!");
                }
            }
            else
            {
                //laserLine.SetPosition(1, child.transform.position + (DICKFocalPoint.transform.position - child.transform.position) * 50);
                //GameObject effect = Instantiate(sparkEffect, particleManager.transform);
                //Debug.Log(hit.transform.position);
                //effect.transform.position = hit.transform.position;
            }
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
