using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitMarker : MonoBehaviour
{
    Image[] hitMarkerImage;
    RectTransform[] hitMarkerTransform;

    float sizeCurrent = 0;
    float sizeMin = 25;
    float sizeMax = 125;
    float sizeIteration = 25;
    float opacityCurrent = 0;
    float opacityMax = 1;
    public float opacityRate = 2;

    void Start()
    {
        Debug.LogError(transform.GetChild(0));

        for (int i = 0; i != 4; i++)
        {
            Debug.LogError(transform.GetChild(i));
            hitMarkerImage[i] = this.transform.GetChild(i).GetChild(0).GetComponent<Image>();
            hitMarkerTransform[i] = hitMarkerImage[i].GetComponent<RectTransform>();

        }

        //hitMarkerImage = this.transform.GetChild(0).GetComponent<Image>();
    }

    void Update()
    {

        opacityCurrent -= opacityRate * Time.deltaTime;
        Debug.Log(opacityCurrent);

        if (opacityCurrent < 0)
        {
            sizeCurrent = 0;
            opacityCurrent = 0;
        }

        //if (sizeCurrent < 0)
        //    sizeCurrent = 0;

        for (int i = 0; i != 4; i++)
        {
            hitMarkerImage[i].color = new Color(1, 1, 1, opacityCurrent);
            hitMarkerTransform[i].sizeDelta = new Vector2(25, sizeCurrent);
        }
      
        if (Input.GetKeyDown(KeyCode.L))
            ActivateHitMarker();
    }

    public void ActivateHitMarker()
    {
        opacityCurrent = opacityMax;
        sizeCurrent += sizeIteration;
        if (sizeCurrent > sizeMax)
            sizeCurrent = sizeMax;
    }
}
