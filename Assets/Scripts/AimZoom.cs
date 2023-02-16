using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimZoom : MonoBehaviour
{
    public float zoom;
    private Camera cam;
    private float Smooth = 5, DefaultZoom = 60f;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        if (cam.fieldOfView > DefaultZoom)
            cam.fieldOfView = DefaultZoom;
    }

    // Update is called once per frame
    public void ZoomIn()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom, Time.deltaTime * Smooth);
    }

    public void ZoomOut()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, DefaultZoom, Time.deltaTime * Smooth);
    }


}
