using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeRolo : WeaponBase
{
    GameObject ui;
    Canvas uiCanvas;

    GameObject uiChamberParent;
    public List<GameObject> uiChamberList = new List<GameObject>();
    public List<Image> uiImageChamberList = new List<Image>();

    public GameObject bulletTrailPF; 

    public enum BulletTypes
    {
        NONE = 0,
        NORMAL = 1,
        SHORTEXPLOSIVE = 2,
        MEDIUMEXPLOSIVE = 3,
        GRAPPLE = 4
    };

    public List<Color> bulletColors = new List<Color>()
    {
        Color.black,
        Color.yellow,
        new Color(255, 60, 0),
        Color.red,
        Color.blue
    };

    public List<BulletTypes> cylinder = new List<BulletTypes>();
    private int cylinderSize = 6;
    void Start()
    {
        base.Start();
        for(int i = 0; i != cylinderSize; i++)
        {
            cylinder.Add(BulletTypes.NONE);
        }

        ui = transform.Find("UI Canvas").gameObject;
        uiCanvas = ui.GetComponent<Canvas>();

        uiChamberParent = ui.transform.Find("Cylinder/Chamber Parent").gameObject;

        for(int i = 0; i != cylinderSize; i++)
        {
            uiChamberList.Add(uiChamberParent.transform.GetChild(i).gameObject);
            uiImageChamberList.Add(uiChamberList[i].GetComponent<Image>());
        }

    }

    void Update()
    {
        base.Update();

        for (int i = 0; i != cylinderSize; i++)
        {
            Debug.Log(i);

            //Debug.Log((int)cylinder[i]);
            Color color = bulletColors[(int)cylinder[i]];
            uiImageChamberList[i].color = color;
        }
    }

    override protected void Fire1Once()
    {
        //if(CheckCanFire(1))
        //{
            Debug.LogError(cylinder[0]);
            switch(cylinder[0])
            {
            case BulletTypes.NORMAL:
                {
                        Ray laserRayCast = new Ray(camera.transform.position + camera.transform.forward * 0.5f, camera.transform.forward);
                        GameObject laser = Instantiate(bulletTrailPF, projectileManager.transform);
                        BallistaLaser laserScript = laser.GetComponent<BallistaLaser>();

                        if (Physics.Raycast(laserRayCast, out RaycastHit hit, 1000))
                        {

                            Vector3 direction = hit.point - bulletEmitter.transform.position;
                            laserScript.InitParticleSystem(bulletEmitter.transform.position, direction, hit.distance);
                            if (hit.collider.transform.tag == "PlayerHitBox")
                            {
                                if (hit.collider.name == "Head")
                                {
                                    particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal, 45);
                                }
                                else
                                {
                                    particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hit.point, hit.normal);

                                }
                                particleManager.GetComponent<ParticleManager>().CreateEffect("ElectricExplosion_PE", hit.point, hit.normal);

                            }
                            else
                            {
                                particleManager.GetComponent<ParticleManager>().CreateEffect("ElectricExplosion_PE", hit.point, hit.normal);

                            }
                        }
                        else
                        {
                         
                            Vector3 direction = camera.transform.forward * 1000 - bulletEmitter.transform.position;
                            laserScript.InitParticleSystem(bulletEmitter.transform.position, camera.transform.forward, 1000);

                        }
                        break;

                }
                    
            }
        //}
    }
}
