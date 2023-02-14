using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public float fireRate = 1;
    public int damage = 1;
    public int bulletsPerShot = 1;
    public float inaccuracy = 10;
    public float projectileVel = 1;

    protected double elapsedSinceLastShot = 0;
    protected double elapsedBetweenEachShot = 0;

    public void Start()
    {
        elapsedBetweenEachShot = 1 / fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Vector3 RandomSpray(Vector3 front, float maxInnacuracy)
    {
        float randomAngle = Random.Range(-maxInnacuracy / 2, maxInnacuracy / 2);
        Vector3 finalVector = Quaternion.AngleAxis(randomAngle, Vector3.right) * front;
        randomAngle = Random.Range(-maxInnacuracy / 2, maxInnacuracy / 2);

        return Quaternion.AngleAxis(randomAngle, Vector3.up) * finalVector;
    }

}
