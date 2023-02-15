using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    private GameObject MinimapCamera;
    // Start is called before the first frame update
    void Start()
    {
        MinimapCamera = GameObject.Find("MinimapCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (MinimapCamera)
        {
            MinimapCamera.transform.position = transform.position;
            MinimapCamera.transform.position = new Vector3(MinimapCamera.transform.position.x, 15f, MinimapCamera.transform.position.z);
        }
    }
}
