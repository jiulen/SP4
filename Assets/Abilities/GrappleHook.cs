using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    public Camera camera;
    private GameObject grappleBody;
    private GameObject hook;
    private GameObject line;
    private GameObject player;
    private Rigidbody grappledRigidBody;
    private FPS playerScript;
    private Rigidbody playerRigidBody;

    private bool hookActive = false;

    public float grappleDuration = 5;
    public float grapplePullForce = 5;

    
    // Keeps track of how long the player has moved in the opposite direction of the hook
    private double grappleMaintainElapsed = 0;
    public float grappleMaintainDuration = 0.4f;
    // Keeps track of if the player has been moving in the opposite direction of the hook since the start of the grapple.
    // As in, it will only be true once the player has move towards the hook for the first frame of each grapple.
    // This is so that the hook will not break if the player was already moving away from the hookPosition from the start of each grapple
    private bool grappleMaintainStarted = false;

    private double grappleElapsed = 0;

    private enum GrappleType
    {
        PULLUSER,
        PULLGRAPPLED,
    }
    private GrappleType grappleType = GrappleType.PULLUSER;

    void Start()
    {
        player = transform.parent.transform.parent.gameObject;
        playerScript = player.GetComponent<FPS>();
        playerRigidBody = player.GetComponent<Rigidbody>();
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

        // Deactiveate grapple after a certain period of time
        grappleElapsed += Time.deltaTime;
        if (grappleElapsed >= grappleDuration)
        {
            SetHookActive(false);
        }

        // Deactiveate grapple after player starts moving away from it
        Vector3 playerToHook = (hook.transform.position - player.transform.position).normalized;
        float dotProduct = Vector3.Dot(player.GetComponent<Rigidbody>().velocity.normalized, playerToHook);

        
        // Player is moving towards
        if(dotProduct > 0)
        {
            grappleMaintainStarted = true;
        }
        // Player is moving towards, plus some leeway to allow the player to pivot around the hook point
        else if (dotProduct > -0.3) 
        {
            grappleMaintainElapsed = 0;
        }
        // Only deactivate the hook when the player is going away from the hookPosition IF the player had already been moving towards the hook since the beginning
        else if (grappleMaintainStarted)
        {
            grappleMaintainElapsed += Time.deltaTime;
            if (grappleMaintainElapsed > grappleMaintainDuration)
            {
                SetHookActive(false);
            }
        }
     

        // Deactivate hook once player is close to it
        if ((hook.transform.position - player.transform.position).magnitude <= 1)
        {
            SetHookActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (hookActive)
            {
                SetHookActive(false);
            }
            else if(playerScript.staminaAmount >= playerScript.staminaGrappleCost)
            {
              

                //Ray laserRayCast = new Ray(body.transform.position, 5 * (hook.transform.position - body.transform.position));
                Ray laserRayCast = new Ray(camera.transform.position,camera.transform.forward);
                Debug.DrawRay(camera.transform.position, 50 * (camera.transform.forward), Color.blue);
                if (Physics.Raycast(laserRayCast, out RaycastHit hit, 100))
                {
                    Debug.LogWarning(hit.rigidbody);
                    hook.transform.SetParent(hit.transform, true);
                    hook.transform.position = hit.point;

                    playerScript.staminaAmount -= playerScript.staminaGrappleCost;
                    Debug.DrawRay(grappleBody.transform.position, 5 * (hook.transform.position - grappleBody.transform.position), Color.red);
                    hook.transform.position = hit.point;
                    SetHookActive(true);
                    grappleElapsed = 0;

                    if (hit.transform.tag == "Player")
                    {
                        grappleType = GrappleType.PULLGRAPPLED;
                        grappledRigidBody = hit.rigidbody;

                    }
                    else
                    {
                        grappleType = GrappleType.PULLUSER;
                        
                    }

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

            LineRenderer grappleLine = line.GetComponent<LineRenderer>();
            grappleLine.positionCount = 2;
            grappleLine.SetPosition(0, grappleBody.transform.position);
            grappleLine.SetPosition(1, hook.transform.position);

            Vector3 playerToHookDirection = (hook.transform.position - player.transform.position).normalized;
            if (grappleType == GrappleType.PULLUSER)
            {
                playerRigidBody.AddForce(playerToHookDirection * grapplePullForce);
                

            }
            else if (grappleType == GrappleType.PULLGRAPPLED)
            { 
                playerRigidBody.AddForce(playerToHookDirection * grapplePullForce / 2);
                grappledRigidBody.AddForce(-playerToHookDirection * grapplePullForce/2);
                grappledRigidBody.useGravity = false;
            }

        }
        else
        {
            hook.SetActive(false);
            line.SetActive(false);
        }
    }

    private void SetHookActive(bool active)
    {
        hookActive = active;
        player.GetComponent<FPS>().isGrapple = active;  
        if(active == true)
        {
            grappleMaintainElapsed = 0;
            grappleMaintainStarted = false;
            //if(player.GetComponent<FPS>().GetIsGrounded())
                //player.GetComponent<Rigidbody>().AddForce(0,100,0);

        }
        else
        {
            hook.transform.parent = this.transform;
            if (grappledRigidBody != null)
                grappledRigidBody.useGravity = true;

        }
    }

}
