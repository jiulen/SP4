using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public GameObject TwoDSprite;
    private GameObject MinimapCamera;
    private GameObject maincam;
    // Start is called before the first frame update
    void Start()
    {
        MinimapCamera = GameObject.Find("MinimapCamera");
        maincam = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {

        MinimapCamera.transform.position = transform.position;
        MinimapCamera.transform.position = new Vector3(MinimapCamera.transform.position.x, 60f, MinimapCamera.transform.position.z);

        Vector3 forward = maincam.transform.forward;
        float targetAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        TwoDSprite.transform.rotation = Quaternion.Euler(90, targetAngle, 0);
    }
}
