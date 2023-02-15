using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    Camera camera;
    Rigidbody rigidbody;
    Transform transform;
    //float angle = 0;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        rigidbody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //angle += 60 * Time.deltaTime;
        //transform.eulerAngles = new Vector3(0,angle,0);

        //Raycasting for camera click movement
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100.0f;
        mousePos = camera.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(camera.transform.position, mousePos - transform.position, Color.red);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // create a plane at 0,0,0 whose normal points to +Y:
        Plane hPlane = new Plane(Vector3.up, Vector3.zero);
        // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
        float distance = 0;
        // if the ray hits the plane...
        if (hPlane.Raycast(ray, out distance))
        {
            // get the hit point:

            transform.position = ray.GetPoint(distance);
        }
    }
}
