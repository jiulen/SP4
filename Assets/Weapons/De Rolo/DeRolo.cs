using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class DeRolo : WeaponBase
{
    GameObject ui;
    Canvas uiCanvas;
    
    GameObject uiChamberParent;
    GameObject uiChamberReloadParent;
    GameObject uiCylinder;

    DeRoloGrappleHook GrappleHookScript;

    public List<Image> uiReloadMarkerList = new List<Image>();
    public List<GameObject> uiChamberList = new List<GameObject>();
    public List<Image> uiImageChamberList = new List<Image>();

    private int activeChamber = 0;
    //public GameObject bulletTrailPF;
    [SerializeField] TrailRenderer bulletTrail;

    public AnimationCurve cycleAnimationCurve = new AnimationCurve();

    public GameObject ExplosiveBulletPF;

    private AmmoWheel ammoWheel;

    [Header("Audio References")]
    public AudioSource AudioNormalFire;
    public AudioSource AudioExplosiveFire;
    public AudioSource AudioReload;
    public AudioSource AudioCycle;


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
    private double reloadElapsed = 0;
    public float reloadDuration = 1f;

    private bool hasUpdatedAfterFireFinished = false;
    private bool hasUpdatedAfterCycledFinished = false;
    private bool hasUpdatedAfterReloadFinished = false;


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
    public int numEmptyChambers = 0;

    public List<BulletTypes> reloadQueue = new List<BulletTypes>();

    private void Awake()
    {
        //owner = transform.parent.parent./*parent.*/gameObject;   // this > right hand > equipped > player
    }

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
        uiChamberReloadParent = uiCylinder.transform.GetChild(1).gameObject;

        for (int i = 0; i != cylinderSize; i++)
        {
            uiChamberList.Add(uiChamberParent.transform.GetChild(i).gameObject);
            uiImageChamberList.Add(uiChamberList[i].GetComponent<Image>());

            // Put it in reverse
            uiReloadMarkerList.Add(uiChamberReloadParent.transform.GetChild(i).GetChild(0).GetComponent<Image>()); // Chamber reload parent > reload pivot > reload marker
        }

        fireAnimation = weaponModel.transform.Find("Revolver").GetComponent<Animator>();
        fireAnimation.speed = 1/* / (float)elapsedBetweenEachShot[1]*/;

        ammoWheel = transform.Find("Ammo Wheel").GetComponent<AmmoWheel>();

        GrappleHookScript = this.GetComponent<DeRoloGrappleHook>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            ui = transform.Find("UI Canvas").gameObject;
            ui.SetActive(true);
        }
    }

    void Update()
    {
        base.Update();

        for (int i = 0; i != cylinderSize; i++)
        {
            Color color = bulletColors[(int)cylinder[i]];
            uiImageChamberList[i].color = color;
        }

        fireCooldownRemaining -= Time.deltaTime;
        cycleElapsed += Time.deltaTime;
        reloadElapsed += Time.deltaTime;

        float anglePerChamber = -360 / cylinderSize;
        float targetAngle = anglePerChamber * activeChamber;
        float currentAngle = targetAngle - anglePerChamber + anglePerChamber * cycleAnimationCurve.Evaluate((float)cycleElapsed/cycleDuration);
        uiCylinder.transform.rotation = Quaternion.Euler(0,0, currentAngle);


        // Reset all reload markers to be invisible
        for (int i = 0; i != cylinderSize; i++)
        {
            uiReloadMarkerList[i].enabled = false;
        }


        // Update reload UI
        if (reloadQueue.Count != 0)
        {
            List<int> emptyChambers = new List<int>();
            // Find empty chambers starting from the active chamber
            // Iterate from the active chamber to the end
            for (int i = activeChamber; i != cylinderSize; i++)
            {
                if (cylinder[i] == BulletTypes.NONE)
                    emptyChambers.Add(i);
            }

            // Circle back to the beginning, and iterate until the chamber before the active chamber is reached
            for (int i = 0; i != activeChamber; i++)
            {
                if (cylinder[i] == BulletTypes.NONE)
                    emptyChambers.Add(i);
            }

            // Color the respective reload markers
            for (int i = 0; i != reloadQueue.Count; i++)
            {
                // If there are more bullets to reload than there are empty chambers, then break
                if (i == emptyChambers.Count)
                    break;

                uiReloadMarkerList[emptyChambers[i]].color = bulletColors[(int)reloadQueue[i]];
                uiReloadMarkerList[emptyChambers[i]].enabled = true;
            }
        }

        UpdateOnceAfterFireFinishes();
        UpdateOnceAfterCycleFinishes();
        UpdateOnceAfterReloadFinishes();
    }

    public void Reload()
    {
        if (!CheckCanFire())
            return;

        AudioReload.Play();

        hasUpdatedAfterReloadFinished = false;
        //cylinder[activeChamber] = BulletTypes.NORMAL;
        reloadElapsed = 0;
        fireAnimation.enabled = true;
        fireAnimation.StopPlayback();
        fireAnimation.StartPlayback();
        fireAnimation.Play("Reload");
        fireAnimation.speed = 1 / reloadDuration;

        List<int> emptyChambers = new List<int>();
        // Find empty chambers starting from the active chamber
        // Iterate from the active chamber to the end
        for (int i = activeChamber; i != cylinderSize; i++)
        {
            if (cylinder[i] == BulletTypes.NONE)
                emptyChambers.Add(i);
        }

        // Circle back to the beginning, and iterate until the chamber before the active chamber is reached
        for (int i = 0; i != activeChamber; i++)
        {
            if (cylinder[i] == BulletTypes.NONE)
                emptyChambers.Add(i);
        }

        // Reload the empty chambers
        for(int i = 0; i != reloadQueue.Count; i++)
        {
            // If there are more bullets to reload than there are empty chambers, then break
            if (i == emptyChambers.Count)
                break;

            cylinder[emptyChambers[i]] = reloadQueue[i];
        }

        reloadQueue.Clear();

        

    }

    // Is called once after the weapon comes off fire cooldown
    private void UpdateOnceAfterFireFinishes()
    {
        if (hasUpdatedAfterFireFinished || fireCooldownRemaining > 0)
            return;

        hasUpdatedAfterFireFinished = true;
        //fireAnimation.enabled = false;
        CycleCylinder();
    }

    // Is called once after the reload is done reloading
    private void UpdateOnceAfterReloadFinishes()
    {
        if (hasUpdatedAfterReloadFinished || reloadElapsed < reloadDuration)
            return;

        hasUpdatedAfterReloadFinished = true;
        fireAnimation.enabled = false;
    }

    // Is called once after the cycling is done cycling
    private void UpdateOnceAfterCycleFinishes()
    {
        if (hasUpdatedAfterCycledFinished || cycleElapsed < cycleDuration)
            return;

        hasUpdatedAfterCycledFinished = true;
        //fireAnimation.enabled = false;
    }

    // We override the base function here as the revolver has some unique properties
    new protected bool CheckCanFire()
    {
        if (fireCooldownRemaining <= 0 && cycleElapsed >= cycleDuration && reloadElapsed >= reloadDuration && !GrappleHookScript.hookActive)
        {
            // fireCooldownRemaining and cycleElapsed to be set outside

            return true;
        }
        return false;
    }

    override protected void Fire1Once()
    {
        if (GrappleHookScript.hookActive)
        {
            GrappleHookScript.SetHookActive(false);
            return;
        }

        if (CheckCanFire() && !ammoWheel.wheelActive)
        {
            //Debug.LogError(activeChamber);
            switch(cylinder[activeChamber])
            {
                case BulletTypes.NORMAL:
                    {
                        int hitType = -1;
                        AudioNormalFire.Play();
                        Ray laserRayCast = new Ray(camera.transform.position + camera.transform.forward * 0.5f, camera.transform.forward);


                        if (Physics.Raycast(laserRayCast, out RaycastHit hit, 1000))
                        {
                            if (hit.collider.transform.tag == "PlayerHitBox")
                            {
                                EntityBase player = hit.collider.gameObject.GetComponent<PlayerHitBox>().owner.GetComponent<EntityBase>();

                                if (player.gameObject == owner)
                                {
                                    if (Physics.Raycast(hit.point + camera.transform.forward * 0.1f, camera.transform.forward, out RaycastHit hit2, 1000))
                                    {

                                        if (hit2.collider.transform.tag == "PlayerHitBox")
                                        {
                                            EntityBase player2 = hit2.collider.gameObject.GetComponent<PlayerHitBox>().owner.GetComponent<EntityBase>();

                                            if (hit2.collider.name == "Head")
                                            {
                                                hitType = 2;
                                                player2.TakeDamage(damage[0] * 2, camera.transform.forward, owner, gameObject);
                                            }
                                            else
                                            {
                                                hitType = 3;
                                                player2.TakeDamage(damage[0], camera.transform.forward, owner, gameObject);
                                            }

                                            CreateTrailServerRpc(hit2.point, hit2.normal, hitType);
                                        }
                                        else
                                        {
                                            hitType = 1;

                                            EntityBase entity2 = hit2.transform.gameObject.GetComponent<EntityBase>();
                                            if (entity2 != null)
                                            {
                                                entity2.TakeDamage(damage[0], camera.transform.forward, owner, gameObject);
                                            }

                                            CreateTrailServerRpc(hit2.point, hit2.normal, hitType);
                                        }
                                    }
                                    else
                                    {
                                        hitType = 0;
                                        CreateTrailServerRpc(bulletEmitter.transform.position + camera.transform.forward * 200, Vector3.zero, hitType);
                                    }
                                }
                                else
                                {
                                    if (hit.collider.name == "Head")
                                    {
                                        hitType = 2;
                                        player.TakeDamage(damage[0] * 2, camera.transform.forward, owner, gameObject);
                                    }
                                    else
                                    {
                                        hitType = 3;
                                        player.TakeDamage(damage[0], camera.transform.forward, owner, gameObject);
                                    }

                                    CreateTrailServerRpc(hit.point, hit.normal, hitType);
                                }
                            }
                            else
                            {
                                hitType = 1;

                                EntityBase entity = hit.transform.gameObject.GetComponent<EntityBase>();
                                if (entity != null)
                                {
                                    entity.TakeDamage(damage[0], camera.transform.forward, owner, gameObject);
                                }

                                CreateTrailServerRpc(hit.point, hit.normal, hitType);
                            }
                        }
                        else
                        {
                            hitType = 0;

                            CreateTrailServerRpc(bulletEmitter.transform.position + camera.transform.forward * 200, Vector3.zero, hitType);
                        }
                    }
                    break;
                case BulletTypes.SHORTEXPLOSIVE:
                    {
                        ShootExplodeBulletServerRpc(0);
                    }
                    break;
                case BulletTypes.MEDIUMEXPLOSIVE:
                    {
                        ShootExplodeBulletServerRpc(1);
                    }
                    break;
                case BulletTypes.GRAPPLE:
                    {
                        GrappleHookScript.FireGrapple();
                    }
                    break;
                case BulletTypes.NONE:
                {
                     CycleCylinder();
                     return;
                }
                break;

            }

            fireCooldownRemaining = elapsedBetweenEachShot[(int)cylinder[activeChamber]];
            fireAnimation.enabled = true;
            fireAnimation.StopPlayback();
            fireAnimation.Play("Fire Revolver");
         
            fireAnimation.speed = 1 / (float)elapsedBetweenEachShot[(int)cylinder[activeChamber]];

            muzzleFlash.GetComponent<ParticleSystem>().Stop();
            muzzleFlash.GetComponent<ParticleSystem>().Play();
            hasUpdatedAfterFireFinished = false;
            numEmptyChambers++;
            cylinder[activeChamber] = BulletTypes.NONE;

           

        }
    }

    // Cycle the cylinder
    private void CycleCylinder()
    {
        AudioCycle.Play();
        hasUpdatedAfterCycledFinished = false;

        fireAnimation.enabled = true;
        fireAnimation.StopPlayback();
        fireAnimation.PlayInFixedTime("Cycle Cylinder",-1,0);
        fireAnimation.speed = 1 / cycleDuration;
        
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


    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint)
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

        Destroy(trail.gameObject, trail.time);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateTrailServerRpc(Vector3 hitPoint, Vector3 hitNormal, int hitType) //Calls the ClientRpc function
    {
        CreateTrailClientRpc(hitPoint, hitNormal, hitType);
    }

    [ClientRpc]
    private void CreateTrailClientRpc(Vector3 hitPoint, Vector3 hitNormal, int hitType) //hitType: 0 - no hit, 1 - hit terrain, 2 - hit player head, 3 - hit player body
    {
        //Make trail renderer
        TrailRenderer trail = Instantiate(bulletTrail, bulletEmitter.transform.position, Quaternion.identity);

        //Spawn bullet tracer
        StartCoroutine(SpawnTrail(trail, hitPoint));

        //Hit particle effects
        switch (hitType)
        {
            case 1:
                particleManager.GetComponent<ParticleManager>().CreateEffect("Sparks_PE", hitPoint, hitNormal);
                break;
            case 2:
                particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hitPoint, hitNormal, 15);
                break;
            case 3:
                particleManager.GetComponent<ParticleManager>().CreateEffect("Blood_PE", hitPoint, hitNormal);
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootExplodeBulletServerRpc(int bulletType) // 0 - SHORTEXPLOSIVE, 1 - MEDIUMEXPLOSIVE
    {
        PlayExplodeSoundClientRpc();

        Transform newTransform = camera.transform;
        Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
        if (bulletType == 0)
        {
            GameObject bullet = Instantiate(ExplosiveBulletPF, bulletEmitter.transform);
            bullet.GetComponent<NetworkObject>().Spawn();
            bullet.GetComponent<NetworkObject>().TrySetParent(projectileManager);
            bullet.GetComponent<ExplosiveBullet>().SetVelocity(front.normalized * 100);
            bullet.GetComponent<ExplosiveBullet>().armingDistance = 0;
            bullet.GetComponent<ExplosiveBullet>().maxDistance = 5;
            bullet.GetComponent<ProjectileBase>().SetObjectReferencesClientRpc(owner.GetComponent<NetworkObject>().NetworkObjectId,
                                                               particleManager.GetComponent<NetworkObject>().NetworkObjectId);
        }
        else if (bulletType == 1)
        {
            Quaternion rotation = Quaternion.LookRotation(front.normalized, Vector3.up);
            GameObject bullet = Instantiate(ExplosiveBulletPF, bulletEmitter.transform.position, rotation);
            bullet.GetComponent<NetworkObject>().Spawn();
            bullet.GetComponent<NetworkObject>().TrySetParent(projectileManager);
            bullet.GetComponent<Rigidbody>().velocity = front.normalized * 50;
            bullet.GetComponent<ProjectileBase>().SetObjectReferencesClientRpc(owner.GetComponent<NetworkObject>().NetworkObjectId,
                                                               particleManager.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    [ClientRpc]
    private void PlayExplodeSoundClientRpc()
    {
        AudioExplosiveFire.Play();
    }
}
