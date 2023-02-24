using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeRolo : WeaponBase
{
    GameObject ui;
    Canvas uiCanvas;

    GameObject uiChamberParent;
    GameObject uiCylinder;

    public List<GameObject> uiChamberList = new List<GameObject>();
    public List<Image> uiImageChamberList = new List<Image>();
    private int activeChamber = 0;
    //public GameObject bulletTrailPF;
    [SerializeField] TrailRenderer bulletTrail;

    private bool hasUpdatedAfterFiredCoolsDown = false;
    private bool hasUpdatedAfterCycledCoolsDown = false;

    public AnimationCurve cycleAnimationCurve = new AnimationCurve();

    public enum BulletTypes
    {
        NONE = 0,
        NORMAL = 1,
        SHORTEXPLOSIVE = 2,
        MEDIUMEXPLOSIVE = 3,
        GRAPPLE = 4
    };

    // Unlike the WeaponBase class, the DeRolo will have one shared weapon cooldown across multiple firing types
    private double fireCooldownRemaining = 0;
    private double cycleElapsed = 0;
    public float cycleDuration = 0.1f;

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

        uiCylinder = ui.transform.Find("Cylinder").gameObject;
        uiChamberParent = uiCylinder.transform.GetChild(0).gameObject;

        for(int i = 0; i != cylinderSize; i++)
        {
            uiChamberList.Add(uiChamberParent.transform.GetChild(i).gameObject);
            uiImageChamberList.Add(uiChamberList[i].GetComponent<Image>());
        }

        fireAnimation = weaponModel.transform.Find("Revolver").GetComponent<Animator>();
        fireAnimation.speed = 1/* / (float)elapsedBetweenEachShot[1]*/;

    }

    void Update()
    {
        base.Update();

        int i = 0;
        int j = 0;
        //// Set the colors after the active chamber, inclusive
        //for (i = activeChamber; i != cylinderSize; i++, j++)
        //{
        //    Color color = bulletColors[(int)cylinder[i]];
        //    uiImageChamberList[j].color = color;
        //}

        //// Set the colors before the active chamber
        //for (i = 0; i != activeChamber; i++, j++)
        //{
        //    Color color = bulletColors[(int)cylinder[i]];
        //    uiImageChamberList[j].color = color;
        //}

        for (i = 0; i != cylinderSize; i++)
        {
            Color color = bulletColors[(int)cylinder[i]];
            uiImageChamberList[i].color = color;
        }

        fireCooldownRemaining -= Time.deltaTime;
        cycleElapsed += Time.deltaTime;

        UpdateOnceAfterFireCoolsDown();
        UpdateOnceAfterCycleCoolsDown();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();   
        }

        float anglePerChamber = -360 / cylinderSize;
        float targetAngle = anglePerChamber * activeChamber;
        float currentAngle = targetAngle - anglePerChamber + anglePerChamber * cycleAnimationCurve.Evaluate((float)cycleElapsed/cycleDuration);
        uiCylinder.transform.rotation = Quaternion.Euler(0,0, currentAngle);
    }

    private void Reload()
    {
        cylinder[activeChamber] = BulletTypes.NORMAL;
    }

    // Is called once after the weapon comes off fire cooldown
    private void UpdateOnceAfterFireCoolsDown()
    {
        if (hasUpdatedAfterFiredCoolsDown || fireCooldownRemaining > 0)
            return;

        hasUpdatedAfterFiredCoolsDown = true;
        fireAnimation.enabled = false;
        CycleCylinder();
    }

    // Is called once after the cycling is done cycling
    private void UpdateOnceAfterCycleCoolsDown()
    {
        if (hasUpdatedAfterCycledCoolsDown || cycleElapsed < cycleDuration)
            return;

        hasUpdatedAfterCycledCoolsDown = true;
        fireAnimation.enabled = false;
    }


    // We override the base function here as the revolver has some unique properties
    new protected bool CheckCanFire()
    {
        if (fireCooldownRemaining <= 0 && cycleElapsed > cycleDuration)
        {
            // fireCooldownRemaining and cycleElapsed to be set outside

            return true;
        }
        return false;
    }

    override protected void Fire1Once()
    {
        if (CheckCanFire())
        {
            Debug.LogError(activeChamber);
            switch(cylinder[activeChamber])
            {
            case BulletTypes.NORMAL:
                {
                    fireCooldownRemaining = elapsedBetweenEachShot[(int)cylinder[activeChamber]];
                    Ray laserRayCast = new Ray(camera.transform.position + camera.transform.forward * 0.5f, camera.transform.forward);
                

                    if (Physics.Raycast(laserRayCast, out RaycastHit hit, 1000))
                    {

                        Vector3 direction = hit.point - bulletEmitter.transform.position;
                        TrailRenderer trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);
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
                        TrailRenderer trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);
                        StartCoroutine(SpawnTrail(trail, bulletEmitter.transform.position + camera.transform.forward * 200, Vector3.zero, null, false));
                    }
                }
                break;

                
                    
            }
            if (cylinder[activeChamber] != BulletTypes.NONE)
            {
                fireAnimation.enabled = true;
                fireAnimation.StopPlayback();
                fireAnimation.Play("Fire Revolver");
                fireAnimation.speed = 1 / (float)elapsedBetweenEachShot[(int)cylinder[activeChamber]];
                hasUpdatedAfterFiredCoolsDown = false;
            }
            cylinder[activeChamber] = BulletTypes.NONE;
        }
    }

    // Cycle the cylinder
    private void CycleCylinder()
    {
        hasUpdatedAfterCycledCoolsDown = false;

        fireAnimation.enabled = true;
        fireAnimation.StopPlayback();
        fireAnimation.Play("Cycle Cylinder");
        fireAnimation.speed = 1;
        
        cycleElapsed = 0;
        activeChamber++;
        if (activeChamber >= cylinderSize)
            activeChamber = 0;
    }

    override protected void Fire2Once()
    {
        if (CheckCanFire())
            CycleCylinder();
    }


    IEnumerator RotateCylinderUI()
    {
        yield return null;

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
