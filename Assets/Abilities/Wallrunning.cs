using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallrunning : MonoBehaviour
{
    LayerMask terrain;
    float wallRunForce = 150;
    float wallCheckDist = 1.0f;
    float wallRunSpeed = 5.0f;
    RaycastHit leftWallHit, rightWallHit;
    bool wallLeft = false, wallRight = false;
    bool wallRunningLeft = false; //check which direction wallrunning on - doesnt matter when not wallrunning

    //Wall jump
    float wallJumpUpForce = 5, wallJumpSideForce = 25;
    float minJumpHeight = 1.5f;

    //Exit wall
    bool exitingWall = false;
    const float exitWallTime = 0.25f;
    float exitWallTimer;

    Rigidbody rb;
    Transform playerBody;
    FPS playerFPSScript;
    Camera playerCamera;

    //Camera effects
    Coroutine zoomInCoroutine, zoomOutCoroutine;
    Coroutine tiltToRightCoroutine, tiltToLeftCoroutine, tiltToMiddleCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerBody = transform.Find("Body pivot").GetChild(0);
        playerFPSScript = GetComponent<FPS>();
        playerCamera = GameObject.Find("Main Camera").GetComponent<Camera>(); //might change to use a game object else to determine orientation

        terrain = 1 << LayerMask.NameToLayer("Terrain");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerFPSScript.canWallrun)
        {
            CheckForWall();
            StateMachine();
        }        
    }

    private void OnDrawGizmos()
    {
        if (playerCamera)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(playerBody.position, playerCamera.transform.right * wallCheckDist);
            Gizmos.DrawRay(playerBody.position, -playerCamera.transform.right * wallCheckDist);
        }
    }

    private void FixedUpdate()
    {
        if (playerFPSScript.isWallrunning)
            WallRunningMovement();
    }

    void CheckForWall()
    {
        wallRight = Physics.Raycast(playerBody.position, playerCamera.transform.right, out rightWallHit, wallCheckDist, terrain);
        wallLeft = Physics.Raycast(playerBody.position, -playerCamera.transform.right, out leftWallHit, wallCheckDist, terrain);
    }

    bool CheckAboveGround()
    {
        return !Physics.Raycast(playerBody.position, Vector3.down, minJumpHeight, terrain);
    }

    void StateMachine()
    {
        //Is Wallrunning
        if ((wallLeft || wallRight) && Input.GetKey(KeyCode.Space) && CheckAboveGround() && !exitingWall)
        {
            if (!playerFPSScript.isWallrunning)
                StartWallRun();
            else
            {
                //Adjust camera tilt here when change direction
                if (wallRunningLeft && wallRight) //From left to right wall
                {
                    //Camera Tilt
                    if (tiltToRightCoroutine != null) StopCoroutine(tiltToRightCoroutine);
                    if (tiltToLeftCoroutine != null) StopCoroutine(tiltToLeftCoroutine);
                    if (tiltToMiddleCoroutine != null) StopCoroutine(tiltToMiddleCoroutine);

                    tiltToLeftCoroutine = StartCoroutine(playerFPSScript.DoTiltZ(5, 20));

                    wallRunningLeft = false;
                }
                else if (!wallRunningLeft && wallLeft) //From right to left wall
                {
                    //Camera Tilt
                    if (tiltToRightCoroutine != null) StopCoroutine(tiltToRightCoroutine);
                    if (tiltToLeftCoroutine != null) StopCoroutine(tiltToLeftCoroutine);
                    if (tiltToMiddleCoroutine != null) StopCoroutine(tiltToMiddleCoroutine);

                    tiltToRightCoroutine = StartCoroutine(playerFPSScript.DoTiltZ(-5, 20));

                    wallRunningLeft = true;
                }
            }
        }
        else if (exitingWall)
        {
            if (playerFPSScript.isWallrunning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }
        //Not wallrunning
        else
        {
            if (playerFPSScript.isWallrunning)
                StopWallRun();
        }

    }

    void StartWallRun()
    {
        playerFPSScript.isWallrunning = true;
        rb.useGravity = false;

        if (wallLeft) wallRunningLeft = true;
        else if (wallRight) wallRunningLeft = false;

        //Camera Zoom
        if (zoomInCoroutine != null) StopCoroutine(zoomInCoroutine);
        if (zoomOutCoroutine != null) StopCoroutine(zoomOutCoroutine);

        zoomOutCoroutine = StartCoroutine(playerFPSScript.DoFOV(75f, 20f));

        //Camera Tilt
        if (tiltToRightCoroutine != null) StopCoroutine(tiltToRightCoroutine);
        if (tiltToLeftCoroutine != null) StopCoroutine(tiltToLeftCoroutine);
        if (tiltToMiddleCoroutine != null) StopCoroutine(tiltToMiddleCoroutine);

        if (wallLeft) tiltToRightCoroutine = StartCoroutine(playerFPSScript.DoTiltZ(-5, 20));
        else if (wallRight) tiltToLeftCoroutine = StartCoroutine(playerFPSScript.DoTiltZ(5, 20));
    }

    void StopWallRun()
    {
        playerFPSScript.isWallrunning = false;
        rb.useGravity = true;

        //Camera Zoom
        if (zoomInCoroutine != null) StopCoroutine(zoomInCoroutine);
        if (zoomOutCoroutine != null) StopCoroutine(zoomOutCoroutine);

        zoomInCoroutine = StartCoroutine(playerFPSScript.DoFOV(60f, 20f));

        //Camera Tilt
        if (tiltToRightCoroutine != null) StopCoroutine(tiltToRightCoroutine);
        if (tiltToLeftCoroutine != null) StopCoroutine(tiltToLeftCoroutine);
        if (tiltToMiddleCoroutine != null) StopCoroutine(tiltToMiddleCoroutine);

        tiltToMiddleCoroutine = StartCoroutine(playerFPSScript.DoTiltZ(0, 20));

        WallJump();
    }

    void WallRunningMovement()
    {
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((playerCamera.transform.forward - wallForward).magnitude > (playerCamera.transform.forward + wallForward).magnitude)
            wallForward = new Vector3(-wallForward.x, wallForward.y, -wallForward.z);

        //forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        float playerVertSpeed = 0;

        if (playerCamera.transform.forward.y > 0)
            playerVertSpeed = 2;
        else if (playerCamera.transform.forward.y > 0)
            playerVertSpeed = -2;

        rb.velocity = new Vector3(rb.velocity.x, playerVertSpeed, rb.velocity.z);

        //Cap speed
        if (rb.velocity.sqrMagnitude > wallRunSpeed * wallRunSpeed)
        {
            rb.velocity = rb.velocity.normalized * wallRunSpeed;
        }
    }

    void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
