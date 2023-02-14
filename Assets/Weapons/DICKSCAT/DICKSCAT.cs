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
    
    public float currentRotate = 0;
    public int numPortals =5;

    private float steamMax = 100;
    private float steamCurrent = 0;
    public float steamRate = 20;

    public GameObject SCATBulletPF;

    
    mode currentMode;
    GameObject portalParent;
    GameObject portal1;
    GameObject DICKFocalPoint;
    GameObject bulletEmitter;
    GameObject bulletEmitterFront;
    GameObject projectileManager;
    Slider steamGauge;
    TMP_Text modeText;
    AudioSource fireAudio;
    void Start()
    {
        base.Start();

        portalParent = GameObject.Find("Portal Parent");
        portal1 = GameObject.Find("Portal Parent/Portal");
        DICKFocalPoint = GameObject.Find("DICK Focal Point");
        bulletEmitter = GameObject.Find("Bullet emitter");
        bulletEmitterFront = GameObject.Find("Bullet emitter front");
        projectileManager = GameObject.Find("Projectile Manager");

        steamGauge = GameObject.Find("UI canvas/Steam Gauge Slider").GetComponent<Slider>();
        modeText = GameObject.Find("UI canvas/Mode text").GetComponent<TMP_Text>();
        fireAudio = GameObject.Find("SampleFire").GetComponent<AudioSource>();

        Debug.Log(DICKFocalPoint);
        for(int i = 0; i != numPortals; i++)
        {

            float angle = (180 / (numPortals - 1)) * i;
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

    private void FixedUpdate()
    {
        
    }

    void Update()
    {
        if (!Input.GetButton("Fire1") || currentMode == mode.SCAT)
        {
            foreach (Transform child in portalParent.transform)
            {
                Transform laser = child.transform.GetChild(0);
                laser.gameObject.SetActive(false);

            }
        }
        if (Input.GetButton("Fire1"))
        {
            switch (currentMode)
            {
                case mode.DICK:
                {
                    //portalParent.GetComponent<Transform>().eulerAngles = new Vector3(0, portalParent.GetComponent<Transform>().eulerAngles.y, currentRotate);
                    
                    steamCurrent += steamRate * Time.deltaTime;
                    if (steamCurrent > steamMax)
                    {
                        steamCurrent = steamMax;
                    }

                    foreach (Transform child in portalParent.transform)
                    {
                        Vector3 midPos = (child.transform.position + DICKFocalPoint.transform.position) / 2;
                        Transform laser = child.transform.GetChild(0);
                         laser.gameObject.SetActive(true);
                        laser.position = midPos;
                        Vector3 direction = child.transform.position - DICKFocalPoint.transform.position;
                        
                        laser.rotation = Quaternion.LookRotation(direction);
                        laser.Rotate(90, 0, 0);

                        Debug.DrawRay(child.transform.position, 5 * (DICKFocalPoint.transform.position - child.transform.position), Color.red);
                        Ray laserRayCast = new Ray(child.transform.position, 5 * (DICKFocalPoint.transform.position - child.transform.position));
                        if (Physics.Raycast(laserRayCast, out RaycastHit hit, 10))
                        {
                            if (hit.collider.tag == "Player")
                            {
                                Debug.Log("HIT!!!");
                            }
                        }
                        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    }
                    break;
                }
                case mode.SCAT:
                {
                    steamCurrent -= steamRate * Time.deltaTime;
                    elapsedSinceLastShot += Time.deltaTime;
                    if (elapsedSinceLastShot >= elapsedBetweenEachShot)
                    {
                        GameObject bullet = Instantiate(SCATBulletPF, bulletEmitter.transform);
                        Vector3 front = bulletEmitterFront.transform.position - bulletEmitter.transform.position;
                        bullet.GetComponent<Rigidbody>().velocity = RandomSpray(front, inaccuracy) * projectileVel;
                        bullet.transform.SetParent(projectileManager.transform);
                        elapsedSinceLastShot = 0;
                        fireAudio.Play();
                    }
                    break;
                }
            }
         
        }
     
        if (Input.GetButtonDown("Fire2"))
        {
            switch(currentMode)
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

        steamGauge.value = steamCurrent;

    }
}
