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
        Ray laserRayCast = new Ray(camera.transform.position,camera.transform.forward);
        GameObject laser = Instantiate(ballistaLaserPF, projectileManager.transform);
        Debug.Log(laser.name);
        BallistaLaser laserScript = laser.GetComponent<BallistaLaser>();
        if (Physics.Raycast(laserRayCast, out RaycastHit hit, 1000))
        {
            laserScript.SetLaserPoins(bulletEmitter.transform.position, hit.point);
        }
        else
        {
            Vector3 test = bulletEmitter.transform.position;
            Debug.Log(test);
            test = camera.transform.position;
            Debug.Log(test);
            test = laser.transform.position;
            Debug.Log(test);
            laserScript.SetLaserPoins(bulletEmitter.transform.position, camera.transform.forward * 1000);

        }
        Debug.DrawRay(camera.transform.position, 50 * (camera.transform.forward), Color.blue);
    

    }

    override protected void Fire2Once()
    {
        Transform newTransform = camera.transform;
        GameObject blast = Instantiate(stormBlastPF, bulletEmitter.transform);
        Vector3 front = newTransform.forward * 1000 - bulletEmitter.transform.position;
        blast.GetComponent<Rigidbody>().velocity = RandomSpray(front.normalized, inaccuracy) * projectileVel;
        blast.transform.SetParent(projectileManager.transform);
        elapsedSinceLastShot = 0;
        fireAudio.Play();
    }

} 
