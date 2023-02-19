using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodEffects : CameraEffects
{
    Color originalAlpha;
    private float originalduration;
    // Start is called before the first frame update
    void Start()
    {
        originalduration = duration;
        base.Start();
        originalAlpha = GetComponent<Image>().color;
        duration = 0;


        Color tempAlpha = GetComponent<Image>().color;
        tempAlpha.a = 0;
        GetComponent<Image>().color = tempAlpha;
    }

    // Update is called once per frame
    void Update()
    {
        if (duration <= 0)
        {
            Color currAlpha = GetComponent<Image>().color;
            currAlpha.a = Mathf.Lerp(currAlpha.a, 0.0f, originalduration * Time.deltaTime);
            GetComponent<Image>().color = currAlpha;
        }
        else
            duration -= Time.deltaTime;
    }

    public void ResetStartDuration()
    {
        duration = originalduration;
        GetComponent<Image>().color = originalAlpha;
    }
}
