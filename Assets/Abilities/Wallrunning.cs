using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Wallrunning : NetworkBehaviour
{
    LayerMask terrain;
    float wallRunForce = 200;
    float wallCheckDist = 2.0f;
    float wallRunSpeed = 7.5f;
    RaycastHit leftWallHit, rightWallHit;
    bool wallLeft = false, wallRight = false;
    bool wallRunningLeft = false; //check which direction wallrunning on - doesnt matter when not wallrunning

    //Wall jump
    float wallJumpUpForce = 5, wallJumpSideForce = 10;
    float minJumpHeight = 1.5f;

    //Exit wall
    bool exitingWall = false;
    const float exitWallTime = 0.5f;
    float exitWallTimer;

    Rigidbody rb;
    Transform playerBody;
    Transform playerHead;
    FPS playerFPSScript;

    //Camera effects
    Coroutine zoomInCoroutine, zoomOutCoroutine;
    Coroutine tiltToRightCoroutine, tiltToLeftCoroutine, tiltToMiddleCoroutine;

    public AudioSource AudioWallrunning;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerFPSScript = GetComponent<FPS>();
        playerBody = playerFPSScript.body.transform;
        playerHead = playerFPSScript.head.transform;

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

    private void FixedUpdate()
    {
        if (playerFPSScript.isWallrunning)
            WallRunningMovement();
    }

    void CheckForWall()
    {
        wallRight = Physics.Raycast(playerBody.position, playerBody.transform.right, out rightWallHit, wallCheckDist, terrain);
        wallLeft = Physics.Raycast(playerBody.position, -playerBody.transform.right, out leftWallHit, wallCheckDist, terrain);
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

                //Use stamina while wallrunning
                playerFPSScript.staminaAmount -= playerFPSScript.staminaWallrunRate * Time.deltaTime;
                //Kick player out of wallrunning when stamina depleted
                if (playerFPSScript.staminaAmount <= 0)
                    StopWallRun();
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
        PlayAudioServerRpc();
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
        StopAudioServerRpc();
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

        if ((playerHead.transform.forward - wallForward).magnitude > (playerHead.transform.forward + wallForward).magnitude)
            wallForward = new Vector3(-wallForward.x, wallForward.y, -wallForward.z);

        //forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        float playerVertSpeed = 0;

        if (playerHead.transform.forward.y > 0)
            playerVertSpeed = 2;
        else if (playerHead.transform.forward.y > 0)
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

    [ServerRpc(RequireOwnership = false)]
    void PlayAudioServerRpc()
    {
        PlayAudioClientRpc();
    }

    [ClientRpc]
    void PlayAudioClientRpc()
    {
        AudioWallrunning.Play();
    }

    [ServerRpc(RequireOwnership = false)]
    void StopAudioServerRpc()
    {
        StopAudioClientRpc();
    }

    [ClientRpc]
    void StopAudioClientRpc()
    {
        AudioWallrunning.Stop();
    }
}
