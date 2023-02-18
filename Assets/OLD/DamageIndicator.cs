using Photon.Chat.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : CameraEffects
{
    private Camera cam;
    private Vector3 sourceDir;
    private float StartDuration = 1.5f;

    public void SetSourcePos(Vector3 dir)
    {
        sourceDir = dir;
    }
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion sourceRot = Quaternion.LookRotation(sourceDir);
        sourceRot.z = -sourceRot.y;
        sourceRot.x = sourceRot.y = 0;
        Vector3 north = new Vector3(0, 0, cam.transform.eulerAngles.y);
        transform.localRotation = sourceRot * Quaternion.Euler(north);

        if (StartDuration <= 0)
        {
            base.Update();

            Color currAlpha = GetComponent<Image>().color;
            currAlpha.a = Mathf.Lerp(currAlpha.a, 0.0f, duration * Time.deltaTime);
            GetComponent<Image>().color = currAlpha;
        }
        StartDuration -= Time.deltaTime;
    }
}
