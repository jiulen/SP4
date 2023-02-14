using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DICKSCAT : WeaponBase
{
    enum mode
    {
        DICK,
        SCAT
    }

    public float DICKDistance = 1;
    public float SCATInnacuracy = 1;
    public float currentRotate = 0;
    public int numPortals =5;

    private float steamMax = 100;
    private float steamCurrent = 0;
    public float steamRate = 20;

    mode currentMode;
    GameObject portalParent;
    GameObject portal1;
    GameObject DICKFocalPoint;
    Slider steamGauge;

    void Start()
    {
        portalParent = GameObject.Find("Portal Parent");
        portal1 = GameObject.Find("Portal Parent/Portal");
        DICKFocalPoint = GameObject.Find("DICK Focal Point");
        steamGauge = GameObject.Find("Steam Gauge Canvas/Steam Gauge Slider").GetComponent<Slider>();

        Debug.Log(DICKFocalPoint);
        for(int i = 0; i != numPortals; i++)
        {

            float angle = (360 / numPortals) * i;
            float portalDistance = 1;
            Vector2 vec2 = MyMath.DegreeToVector2(angle);
            Vector3 vec3 = new Vector3(vec2.x, vec2.y, 0);
            GameObject portal;

            if(i == 0)
            {
                portal = portal1;
            }
            else
            {
                portal = Instantiate(portal1, portalParent.transform);
                portal.transform.parent = portalParent.transform;
                portal.transform.localPosition = new Vector3(0,0,0);
                //portal.transform.position = new Vector3(0, 0, 0);

                //portal.transform.position.Set(0, 0, 0);
                //Debug.Log(portal.transform.localPosition);
                //Debug.Log(i.ToString() + portal.transform.position);
            }

            portal.transform.Translate(vec3);
        }

        DICKFocalPoint.transform.Translate(0,0,DICKDistance);

        currentMode = mode.DICK;
    }

    void Update()
    {

        if (Input.GetButton("Fire1"))
        {
            currentRotate += 180 * Time.deltaTime;
            portalParent.GetComponent<Transform>().eulerAngles = new Vector3(0, portalParent.GetComponent<Transform>().eulerAngles.y, currentRotate);
            steamCurrent += steamRate * Time.deltaTime;
        }

        foreach (Transform child in portalParent.transform)
        {
            Vector3 midPos = (child.transform.position + DICKFocalPoint.transform.position) / 2;
            Debug.Log(midPos);
            //child.transform.GetChild(0).transform.position = midPos;
            Debug.DrawRay(child.transform.position, 5*(DICKFocalPoint.transform.position - child.transform.position), Color.red);
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        steamGauge.value = steamCurrent;

    }
}
