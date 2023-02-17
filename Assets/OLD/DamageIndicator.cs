using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : CameraEffects
{
    private Camera cam;
    public Vector3 sourcePosition;
    private GameObject source;

    public void SetSource(GameObject obj)
    {
        source = obj;
    }
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        transform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = cam.transform.position - sourcePosition;
        Quaternion sourceRot = Quaternion.LookRotation(forward);
        sourceRot.z = sourceRot.y;
        sourceRot.x = sourceRot.y = 0;
        Vector3 north = new Vector3(0, 0, cam.transform.eulerAngles.y);
        transform.localRotation = sourceRot * Quaternion.Euler(north);

        base.Update();
    }
}
