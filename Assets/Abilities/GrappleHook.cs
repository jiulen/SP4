using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    private Camera camera;
    private GameObject body;
    private GameObject hook;
    private GameObject line;
    private GameObject player;

    private bool hookActive;
    private Vector3 hookPosition;

    public float grappleDuration = 2;
    private double grappleElapsed = 0;
    void Start()
    {
        camera = transform.Find("/Main Camera").GetComponent<Camera>();
        body = GameObject.Find("Body");
        hook = GameObject.Find("Hook");
        line = GameObject.Find("Line");
        player = transform.parent.transform.parent.transform.Find("Player Entity").gameObject;
        GameObject test = transform.parent.gameObject;
        GameObject test2 = test.transform.parent.gameObject;
        GameObject test3 = test2.transform.Find("Player Entity").gameObject;
        //Debug.Log("Hello everynyan" + test3.name);
   
    }

    // Update is called once per frame
    void Update()
    {
        grappleElapsed += Time.deltaTime;
        if (grappleElapsed >= grappleDuration)
        {
            hookActive = false;
        }

        hook.transform.position = hookPosition;

        if ((hook.transform.position - player.transform.position).magnitude <= 1)
        {
            hookActive = false;
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if (hookActive)
            {
                hookActive = false;
            }
            else
            {
                Debug.DrawRay(body.transform.position, 5 * (hook.transform.position - body.transform.position), Color.red);

                //Ray laserRayCast = new Ray(body.transform.position, 5 * (hook.transform.position - body.transform.position));
                Ray laserRayCast = new Ray(camera.transform.position, 50 * (camera.transform.forward));
                Debug.DrawRay(camera.transform.position, 50 * (camera.transform.forward), Color.blue);
                if (Physics.Raycast(laserRayCast, out RaycastHit hit, 50))
                {
                    hookPosition = hit.point;
                    hookActive = true;
                    grappleElapsed = 0;    
                }
            }
          
        }

      

    }

    void LateUpdate()
    {
        if (hookActive)
        {
            hook.SetActive(true);
            line.SetActive(true);
            //player.GetComponent<Rigidbody>().velocity = ((hook.transform.position - player.transform.position).normalized * 30 );
            player.GetComponent<Rigidbody>().AddForce((hook.transform.position - player.transform.position).normalized * 30);

            LineRenderer grappleLine = line.GetComponent<LineRenderer>();
            grappleLine.positionCount = 2;
            grappleLine.SetPosition(0, body.transform.position);
            grappleLine.SetPosition(1, hook.transform.position);
        }
        else
        {
            hook.SetActive(false);
            line.SetActive(false);
        }
    }
}
