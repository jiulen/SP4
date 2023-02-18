using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    public Camera camera;
    private GameObject grappleBody;
    private GameObject hook;
    private GameObject line;
    public GameObject player;

    private bool hookActive;
    private Vector3 hookPosition;

    public float grappleDuration = 2;
    public float grapplePullForce = 15;

    // Once the player enters this radius, the grapple will unhook if they exit it. It is so that the player can be slingshot without having to cancel the grapple hook themselves
    public float grappleMaintainDistance = 1;
    private bool grappleMaintainEntered = false;

    private double grappleElapsed = 0;
    void Start()
    {
        player = transform.parent.transform.parent.gameObject;
        camera = player.GetComponent<FPS>().camera;
        grappleBody = this.transform.Find("Grapple Body").gameObject;
        hook = this.transform.Find("Hook").gameObject;
        line = this.transform.Find("Line").gameObject;

        //GameObject test = transform.parent.gameObject;
        //GameObject test2 = test.transform.parent.gameObject;
        //GameObject test3 = test2.transform.Find("Player Entity").gameObject;
        //Debug.Log("Hello everynyan" + test3.name);
   
    }

    // Update is called once per frame
    void Update()
    {
        if ((player.transform.position - hookPosition).magnitude <= grappleMaintainDistance)
        {
            grappleMaintainEntered = true;
        }
        else if (grappleMaintainEntered)
        {
            grappleMaintainEntered = false;
            hookActive = false;
        }


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
                Debug.DrawRay(grappleBody.transform.position, 5 * (hook.transform.position - grappleBody.transform.position), Color.red);

                //Ray laserRayCast = new Ray(body.transform.position, 5 * (hook.transform.position - body.transform.position));
                Ray laserRayCast = new Ray(camera.transform.position,camera.transform.forward);
                Debug.DrawRay(camera.transform.position, 50 * (camera.transform.forward), Color.blue);
                if (Physics.Raycast(laserRayCast, out RaycastHit hit, 100))
                {
                    hookPosition = hit.point;
                    hookActive = true;
                    grappleElapsed = 0;
                    grappleMaintainEntered = false;

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
            player.GetComponent<Rigidbody>().AddForce((hook.transform.position - player.transform.position).normalized * grapplePullForce);

            LineRenderer grappleLine = line.GetComponent<LineRenderer>();
            grappleLine.positionCount = 2;
            grappleLine.SetPosition(0, grappleBody.transform.position);
            grappleLine.SetPosition(1, hook.transform.position);
        }
        else
        {
            hook.SetActive(false);
            line.SetActive(false);
        }
    }
}
