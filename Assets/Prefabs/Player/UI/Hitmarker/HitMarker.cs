using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitMarker : MonoBehaviour
{

    public List<Image> hitMarkerImage = new List<Image>();
    public List<RectTransform> hitMarkerTransform = new List<RectTransform>();

    float sizeCurrent = 0;
    float sizeMin = 25;
    float sizeMax = 45;
    float sizeIteration = 5;
    float opacityCurrent = 0;
    float opacityMax = 1;
    public float opacityRate = 2;

    public AudioSource AudioNormalHit;
    public AudioSource AudioCriticalHit;

    private Color currentColor = new Color(1,1,1,0);

    void Start()
    {
        for (int i = 0; i != 4; i++)
        {
            hitMarkerImage.Add(this.transform.GetChild(i).GetChild(0).GetComponent<Image>());
            hitMarkerTransform.Add(hitMarkerImage[i].GetComponent<RectTransform>());

        }

        //hitMarkerImage = this.transform.GetChild(0).GetComponent<Image>();
    }

    void Update()
    {

        opacityCurrent -= opacityRate * Time.deltaTime;

        if (opacityCurrent < 0)
        {
            sizeCurrent = sizeMin;
            opacityCurrent = 0;
        }

        //if (sizeCurrent < 0)
        //    sizeCurrent = 0;

        for (int i = 0; i != 4; i++)
        {
            currentColor.a = opacityCurrent;
            hitMarkerImage[i].color = currentColor;
            hitMarkerTransform[i].sizeDelta = new Vector2(25, sizeCurrent);
        }
      
        if (Input.GetKeyDown(KeyCode.L))
            ActivateHitMarker(false);

        if (Input.GetKeyDown(KeyCode.K))
            ActivateHitMarker(true);

    }

    public void ActivateHitMarker(bool isCritical)
    {
        opacityCurrent = opacityMax;
        if (isCritical)
        {
            sizeCurrent = sizeMax;
            currentColor = new Color(1, 0, 0, 1);
            AudioCriticalHit.Play();
        }
        else
        {
            currentColor = new Color(1, 1, 1, 1);

            sizeCurrent += sizeIteration;
            if (sizeCurrent > sizeMax)
                sizeCurrent = sizeMax;

            AudioNormalHit.Play();
        }
    }
}
