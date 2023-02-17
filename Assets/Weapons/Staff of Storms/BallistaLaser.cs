using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallistaLaser : ProjectileBase
{
    LineRenderer line;
    GameObject particleObject;
    ParticleSystem particleSystem;

    void Start()
    {
      
    }

    private void Awake()
    {
        base.Start();
        line = this.gameObject.GetComponent<LineRenderer>();
        particleObject = this.transform.GetChild(0).gameObject;
        particleSystem = particleObject.GetComponent<ParticleSystem>();
        Debug.Log(line.name);
    }
    public void InitParticleSystem(Vector3 start, Vector3 direction, float distance)
    {
        direction.Normalize();

        if (distance > 100) // Limit distance so not too many particles are generated
        {
            Vector3 end = start + (direction * distance);
            Vector3 newDirection = (end - start).normalized;
            distance = 100;
            Vector3 newEnd = start + newDirection * distance;
            Vector3 newPos = (start + newEnd) / 2;
            particleObject.transform.position = newPos;

        }
        else
        {
            Vector3 end = start + (direction * distance);
            Vector3 midPos = (start + end) / 2;
            particleObject.transform.position = midPos;
        }

        particleObject.transform.rotation = Quaternion.LookRotation(direction);

        //float speed = particleObject.GetComponent<ParticleSystem>().main.startSpeed.constant;
        //float duration = distance / speed;
        particleSystem.Stop();
        var shape = particleSystem.shape;
     
        shape.radius = distance/ 2;
        var burst = particleSystem.emission;
        float particleAmt = distance * 100; // Recommended particle density of 100 per 1 unit of radius length
        ParticleSystem.Burst newBurst = new ParticleSystem.Burst(0, particleAmt);
        burst.SetBurst(0, newBurst); 
        particleSystem.Play();
    }

    // Deprecated line render based laser for particle based
    //public void SetLaserPoins(Vector3 start, Vector3 end)
    //{
    // 
    //    Debug.Log(line.name);





    //    line.positionCount = 2;
    //    line.SetPosition(0, start);
    //    line.SetPosition(1, end);

    //}
    // Update is called once per frame
    void Update()
    {
        base.Update();
        float width = (float)(duration - elapsed) / duration;
        line.SetWidth(width, width);
    }
}
