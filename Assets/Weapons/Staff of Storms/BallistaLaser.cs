using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallistaLaser : ProjectileBase
{
    LineRenderer line;
    void Start()
    {
      
    }

    private void Awake()
    {
        base.Start();
        line = this.gameObject.GetComponent<LineRenderer>();
        Debug.Log(line.name);
    }

    public void SetLaserPoins(Vector3 start, Vector3 end)
    {


        Debug.Log(line.name);





        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

    }
    // Update is called once per frame
    void Update()
    {
        base.Update();
        float width = (float)(duration - elapsed) / duration;
        line.SetWidth(width, width);
    }
}
