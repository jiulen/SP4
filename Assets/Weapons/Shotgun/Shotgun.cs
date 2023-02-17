using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shotgun : WeaponBase
{
    override protected void Fire1()
    {
        if (CheckCanFire(1))
        {
            Transform newTransform = camera.transform;
            for (int bullets = 0; bullets < bulletsPerShot[0]; ++bullets)
            {
                Vector3 bulletDir = newTransform.forward;
                bulletDir = RandomSpray(bulletDir.normalized, inaccuracy[0]);
                //Do hitscan
                //Do bullet tracer
            }
        }
    }
}
