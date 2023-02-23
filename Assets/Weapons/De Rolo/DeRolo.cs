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
    private int activeChamber = 0;
    //public GameObject bulletTrailPF;
    [SerializeField] TrailRenderer bulletTrail;

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
        for(int i = 0; cylinder.Count != cylinderSize; i++)
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

        int i = 0;
        int j = 0;
        // Set the colors after the active chamber, inclusive
        for (i = activeChamber; i != cylinderSize; i++, j++)
        {
            Color color = bulletColors[(int)cylinder[i]];
            uiImageChamberList[j].color = color;
        }

        // Set the colors before the active chamber
        for (i = 0; i != activeChamber; i++, j++)
        {
            Color color = bulletColors[(int)cylinder[i]];
            uiImageChamberList[j].color = color;
        }

    }

    override protected void Fire1Once()
    {
        if (CheckCanFire(1))
        {
            Debug.LogError(activeChamber);
            switch(cylinder[activeChamber])
            {
            case BulletTypes.NORMAL:
                {
                    Ray laserRayCast = new Ray(camera.transform.position + camera.transform.forward * 0.5f, camera.transform.forward);
                    //GameObject laser = Instantiate(bulletTrailPF, projectileManager.transform);
                    //BallistaLaser laserScript = laser.GetComponent<BallistaLaser>();

                    if (Physics.Raycast(laserRayCast, out RaycastHit hit, 1000))
                    {

                        Vector3 direction = hit.point - bulletEmitter.transform.position;
                        //laserScript.InitParticleSystem(bulletEmitter.transform.position, direction, hit.distance);
                        //Do bullet tracer (if hit)
                        TrailRenderer trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);
                        //Spawn bullet tracer
                        StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, hit.collider, true));


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
                            particleManager.GetComponent<ParticleManager>().CreateEffect("Sparks_PE", hit.point, hit.normal);

                        }
                        else
                        {
                            particleManager.GetComponent<ParticleManager>().CreateEffect("Sparks_PE", hit.point, hit.normal);

                        }
                    }
                    else
                    {

                        Vector3 direction = camera.transform.forward * 1000 - bulletEmitter.transform.position;
                        //laserScript.InitParticleSystem(bulletEmitter.transform.position, camera.transform.forward, 1000);

                        //Do bullet tracer (if hit)

                        TrailRenderer trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);
                        //Spawn bullet tracer
                        StartCoroutine(SpawnTrail(trail, bulletEmitter.transform.position + camera.transform.forward * 200, Vector3.zero, null, false));

                    }
                }
                break;

                
                    
            }

            // Cycle the cylinder
            activeChamber++;
            if (activeChamber >= cylinderSize)
                activeChamber = 0;
        }
    }

    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal, Collider hitCollider, bool madeImpact)
    {
        Vector3 startPos = trail.transform.position;

        float distance = Vector3.Distance(trail.transform.position, hitPoint);
        float startDistance = distance;

        while (distance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPos, hitPoint, 1 - (distance / startDistance));
            distance -= Time.deltaTime * 400;

            yield return null;
        }

        trail.transform.position = hitPoint;

        if (madeImpact && hitCollider)
        {
            //do hit effects here
        }

        Destroy(trail.gameObject, trail.time);
    }
}
