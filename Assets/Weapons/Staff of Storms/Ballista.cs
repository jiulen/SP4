using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballista : WeaponBase
{
    public GameObject ballistaLaserPF;
    public GameObject stormBlastPF;
    void Start()
    {
        base.Start();
        bulletEmitter = GameObject.Find("Gun/Bullet emitter");
       
    }

    void Update()
    {
        base.Update();
    }

    override protected void Fire1Once()
    {
        if (CheckCanFire(0))
        {
            Ray laserRayCast = new Ray(camera.transform.position, camera.transform.forward);
            GameObject laser = Instantiate(ballistaLaserPF, projectileManager.transform);
            BallistaLaser laserScript = laser.GetComponent<BallistaLaser>();
            if (Physics.Raycast(laserRayCast, out RaycastHit hit, 1000))
            {
                laserScript.SetLaserPoins(bulletEmitter.transform.position, hit.point);
            }
            else
            {
                Vector3 test = bulletEmitter.transform.position;
                test = camera.transform.position;
                test = laser.transform.position;
                laserScript.SetLaserPoins(bulletEmitter.transform.position, camera.transform.forward * 1000);

            }
            Debug.DrawRay(camera.transform.position, 50 * (camera.transform.forward), Color.blue);
        }

    }

    override protected void Fire2Once()
    {
        Transform newTransform = camera.transform;
        GameObject blast = Instantiate(stormBlastPF, bulletEmitter.transform);
        Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
        blast.GetComponent<Rigidbody>().velocity = front.normalized * projectileVel[1];
        blast.transform.SetParent(projectileManager.transform);
        blast.GetComponent<StormBlast>().SetCreator(playerOwner);
        fireAudio.Play();
    }

} 
