using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Threading;

public class FPS : NetworkBehaviour
{
    public CharacterController con;
    [SerializeField] CapsuleCollider capsuleCollider;
    public Camera camera; // Main camera
    Vector3 moveVector;
    private bool isGround = false;
    private float pitch, yaw, roll;
    private float CamSen;
    private float speed = 5f;
    private float DashSpeed = 30f, DashForwardVelocity, DashTime = 0.5f;
    private float decel;

    private Rigidbody rigidbody;
    // general
    private GameObject body;
    //private GameObject head;
    private GameObject bodyPivot;
    // General
    public float airMovementMultiplier = 2.5f;

    // Grounded check
    private float headHeight = 1;
    private float bodyHeight = 1;
    private float bodyRadius = 1;

    // Jump
    private bool doublejump = false;
    public float jumpForce = 250;

    // Dash
    public float dashDuration = 0.2f;
    float dashProgress = 0.2f;
    public bool candash = true;
    public int dashNum = 3;

    private float dashMetre = 0;
    public float dashMetreRate = 30;
    private float dashMetreMax = 100;
    private Vector3 storeDashDir;

    // Slide
    public float slideMultiplier = 2;
    private Vector3 storeSlideDir;
    private bool isSlide = false;

    Canvas uiCanvas;

    //// Gravity
    //float gravity = -9.81f;
    //Vector3 velocity;

    // Drop kick
    bool dropKickActive = false;

    // Teleport
    bool canTeleport = true;

    enum TeleportStates
    {
        NONE,
        TELEPORT_MARKER,
        TELEPORT_CHANNEL,
    };
    TeleportStates teleportState = TeleportStates.NONE;
    const float teleportDuration = 1.0f, teleportCooldown = 1.0f;
    float teleportProgress = 0.0f, teleportCooldownTimer = teleportCooldown;
    float teleportDistance = 5.0f;
    float tpVerticalOffset = 0;
    [SerializeField] GameObject tpMarkerPrefab;
    GameObject tpMarker;
    MeshRenderer tpMarkerMR;
    LayerMask tpLayerMask;

    private Transform currentEquipped;

    enum Dash
    {
        NONE,
        DASH
    }

    Dash dashstate = Dash.NONE;

    //Wallrunning
    public bool canWallrun = true;
    public bool isWallrunning = false;

    void Start()
    {
        uiCanvas = GameObject.Find("Player System/Canvas").GetComponent<Canvas>();

        bodyPivot = transform.Find("Body pivot").gameObject;
        body = bodyPivot.transform.GetChild(0).gameObject;
        //body = transform.Find("Body").gameObject;
        capsuleCollider = this.GetComponent<CapsuleCollider>(); //set in editor

        headHeight = this.transform.position.y - body.transform.position.y + (body.GetComponent<CapsuleCollider>().height * this.transform.localScale.y * body.transform.localScale.y)/2;

        // bodyHeight refers to its relative y position, not the height of the collider
        bodyHeight = (body.GetComponent<CapsuleCollider>().height * this.transform.localScale.y * body.transform.localScale.y)/ 2;
        bodyRadius = (body.GetComponent<CapsuleCollider>().radius * this.transform.localScale.y * body.transform.localScale.y);
        //Set dash progress to more than dash so dash isn't activated on start
        dashProgress = dashDuration + 1;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        pitch = yaw = roll =  0f;
        CamSen = 220f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        tpMarker = Instantiate(tpMarkerPrefab);
        tpMarkerMR = tpMarker.GetComponent<MeshRenderer>();
        tpMarkerMR.enabled = false;
        tpVerticalOffset = capsuleCollider.height * 0.5f - tpMarker.transform.localScale.y; //do this whenever player rigidbody scale changes

        currentEquipped = transform.parent.Find("Equipped");
        transform.position = new Vector3(transform.position.x, 2.0f, transform.position.z);
        rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.velocity.Set(0, 0, 0);


        tpLayerMask = 1 << LayerMask.NameToLayer("Terrain"); //use later when got structures in level
    }


    // Update is called once per frame
    void Update()
    {
        // Reset position and velocity if player goes out of bounds for debugging
        if(transform.position.magnitude > 100 || transform.position.y <= -20)
        {
            transform.position = new Vector3(0, 10, 0);
            rigidbody.velocity = new Vector3(0, 0, 0);
        }
        UpdateGrounded();

        //if (!IsOwner) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float playerVerticalInput = Input.GetAxisRaw("Vertical"); // 1: W key , -1: S key, 0: no key input
        float playerHorizontalInput = Input.GetAxisRaw("Horizontal");

        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;


        yaw += mouseX * CamSen * Time.deltaTime;
        pitch -= mouseY * CamSen * Time.deltaTime;

        //if (!isWallrunning)
        moveVector = (playerVerticalInput * forward) + (playerHorizontalInput * right);
        moveVector.Normalize();

        // WASD movement
        if (isGround)
        {
            //moveVector = (playerVerticalInput * forward) + (playerHorizontalInput * right);
            //moveVector.Normalize();
            rigidbody.velocity = moveVector * speed;

        }
        else
        {
            rigidbody.AddForce(moveVector * airMovementMultiplier);
            //isSlide = false;
            //this.transform.position = new Vector3(this.transform.position.x, headHeight, this.transform.position.z);

        }


        // Drop kick and slide
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isGround)
            {
                if (isSlide)
                {
                    isSlide = false;
                    this.transform.position = new Vector3(this.transform.position.x, headHeight, this.transform.position.z);

                }
                else
                {
                    isSlide = true;
                    storeSlideDir = moveVector;

                }
            }
            else
                dropKickActive = true;
        }


            // Drop kick
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                dropKickActive = true;
            }

            if (dropKickActive)
            {
                if (isGround)
                    dropKickActive = false;
                else
                    rigidbody.velocity = new Vector3(0, -50, 0);
            }
            //if (isGround && moveVector.magnitude == 0)
            //{
            //    //rigidbody.velocity = new Vector3(0, 0, 0);
            //    rigidbody.AddForce(-rigidbody.velocity.normalized * speed);

            //}
            //else
            //{
            //    Debug.Log("ruh roh");
            //    rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
            //    rigidbody.AddForce(moveVector * 100);
            //}
            //this.GetComponent<Rigidbody>().velocity = moveVector;


        


        //Rotation
        pitch = Mathf.Clamp(pitch, -85f, 85f);
        float targetAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        camera.transform.rotation = Quaternion.Euler(pitch, yaw, roll);
        //transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        //Position & gravity
        //con.Move(moveVector * speed * Time.deltaTime);
        //con.Move(this.GetComponent<Rigidbody>().velocity * Time.deltaTime);

        camera.transform.position = transform.position;
        Sniper sniper = transform.parent.GetComponentInChildren<Sniper>();
        if (sniper != null && sniper.allowbobbing)
        {
            camera.transform.position += camera.transform.up * (Mathf.Sin(sniper.stablizeElasped * 2) / 2) * 0.4f + camera.transform.right * Mathf.Cos(sniper.stablizeElasped) * 0.4f;
        }

        Jump();

        // Limit speed
        Vector3 velocityWithoutY = rigidbody.velocity;
        velocityWithoutY.y = 0;
        if (velocityWithoutY.magnitude >= speed)
        {
            velocityWithoutY = velocityWithoutY.normalized * speed;
            velocityWithoutY.y = rigidbody.velocity.y;
            //rigidbody.velocity = velocityWithoutY.normalized * 5;
            rigidbody.velocity = velocityWithoutY;
        }

        UpdateDash();
        UpdateSlide();
        //con.Move(velocity * Time.deltaTime);
        //velocity.y += gravity * Time.deltaTime;

        camera.transform.position = this.transform.position;

        if (canTeleport) UpdateTeleport();

        currentEquipped.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        currentEquipped.transform.position = this.transform.position;
        //Debug.Log(targetAngle);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (!IsOwner) return;

    }

    private void Jump()
    {
        //Ground now has a dedicated function called at the beginning of Update()

        //RaycastHit raycasthit;
        //Ray ray = new Ray(transform.position, -transform.up);

        //if (Physics.Raycast(ray, out raycasthit, (GetComponent<CapsuleCollider>().height / 2) + 0.1f))
        //{
        //    isGround = true;
        //}
        //else
        //{
        //    isGround = false;
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGround)
            {

                rigidbody.AddForce(0, jumpForce, 0);
                //velocity.y = Mathf.Sqrt(300 * Time.deltaTime * -2f * gravity);
                doublejump = true;
                //if(isSlide)
                //{
                //    isSlide = false;
                //    this.transform.position = new Vector3(this.transform.position.x, headHeight, this.transform.position.z);
                //}
            }
            else if (doublejump)
            {
                rigidbody.AddForce(0, jumpForce, 0);
                //velocity.y = Mathf.Sqrt(300 * Time.deltaTime * -2f * gravity);
                doublejump = false;
            }
        }

    }

    private void StartTeleport()
    {
        switch (teleportState)
        {
            case TeleportStates.NONE:
                {
                    teleportState = TeleportStates.TELEPORT_MARKER;
                    tpMarkerMR.enabled = true;

                    break;
                }
            case TeleportStates.TELEPORT_MARKER:
                {
                    teleportProgress = 0.0f;
                    teleportState = TeleportStates.TELEPORT_CHANNEL;

                    Vector3 forward = camera.transform.forward;
                    forward.y = 0;
                    forward.Normalize();

                    break;
                }
        }
    }

    private void UpdateTeleport()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && teleportState < TeleportStates.TELEPORT_CHANNEL && teleportCooldownTimer >= teleportCooldown)
            StartTeleport();

        if (teleportState == TeleportStates.TELEPORT_MARKER)
        {
            Vector3 forward = camera.transform.forward.normalized;

            Vector3 tpMarkerPos;

            //CapsuleCast forward
            RaycastHit raycastHitForward;
            Vector3 bottomCenter = transform.position - Vector3.up * capsuleCollider.height * 0.5f;
            Vector3 topCenter = bottomCenter + Vector3.up * capsuleCollider.height;

            if (Physics.CapsuleCast(bottomCenter, topCenter, capsuleCollider.radius, forward, out raycastHitForward, teleportDistance + transform.localScale.x * 0.25f))
            {
                tpMarkerPos = transform.position + forward * (raycastHitForward.distance - transform.localScale.x * 0.25f);
            }
            else
            {
                tpMarkerPos = transform.position + forward * teleportDistance;
            }

            //Raycast down
            RaycastHit raycastHitDown;
            Ray rayDown = new Ray(tpMarkerPos, -transform.up);

            if (Physics.Raycast(rayDown, out raycastHitDown, 11.0f))
            {
                tpMarker.transform.position = raycastHitDown.point + new Vector3(0, tpMarker.transform.localScale.y, 0);
            }
            else
            {
                Debug.Log("TP Raycast didnt hit!");
            }
        }
        else if (teleportState == TeleportStates.TELEPORT_CHANNEL)
        {
            if (teleportProgress < teleportDuration)
            {
                //Channel teleport
                teleportProgress += Time.deltaTime;
            }
            else
            {
                //Do teleport
                transform.position = tpMarker.transform.position + new Vector3(0, tpVerticalOffset, 0);
                camera.transform.position = transform.position;

                teleportState = TeleportStates.NONE;
                tpMarkerMR.enabled = false;
                teleportCooldownTimer = 0;
            }
        }
        else
        {
            teleportCooldownTimer += Time.deltaTime;
        }
    }

    private void UpdateDash()
    {
        //Debug.Log(dashMetre);
        //Debug.Log(dashMetreMax);
        //Debug.Log(dashNum);
        dashProgress += Time.deltaTime;

        dashMetre += dashMetreRate * Time.deltaTime;
        if(dashMetre > dashMetreMax)
        {
            dashMetre = dashMetreMax;
        }

        //Update the UI
    

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashMetre >= dashMetreMax / dashNum && candash)
        {
            // If no keyboard input, use camera direction
            if (moveVector.magnitude == 0)
            {
                Vector3 forward = camera.transform.forward;
                forward.y = 0;
                forward.Normalize();
                storeDashDir = forward;
                dashProgress = 0;
                dashMetre -= (float)(dashMetreMax / dashNum);
            }
            // Otherwise, dash towards movement input
            else
            {
                storeDashDir = moveVector;
                dashProgress = 0;
                dashMetre -= (float)(dashMetreMax / dashNum);
            }
        }
        if (dashProgress < dashDuration)
        {
            // If the player is falling, cancel their vertical velocity
            if (rigidbody.velocity.y < 0)
                rigidbody.velocity = storeDashDir * speed * 3;
            // If the player is jumping, maintain that velocity
            else
            {
                Vector3 newVelocity = storeDashDir * speed * 3;
                newVelocity.y = rigidbody.velocity.y;
                rigidbody.velocity = newVelocity;
            }
        }

        int i = 0;
        foreach (Transform child in uiCanvas.transform)
        {
            Slider slider = child.GetComponent<Slider>();
            slider.maxValue = dashMetreMax / dashNum;
            float segmentedValue = dashMetre - (dashMetreMax / dashNum) * i;
            slider.value = segmentedValue;
            i++;

        }

        return;
    }

    private void UpdateSlide()
    {
        if (isSlide)
        {
            Vector3 newVel = storeSlideDir.normalized * speed * slideMultiplier; ;
            newVel.y = rigidbody.velocity.y;
            rigidbody.velocity = newVel;
            this.transform.localRotation = Quaternion.Euler( -45, this.transform.localRotation.y, this.transform.localRotation.z);
            bodyPivot.transform.localRotation = Quaternion.Euler(-45, bodyPivot.transform.localRotation.y, bodyPivot.transform.localRotation.z);

        }
        else
        {
            this.transform.localRotation = Quaternion.Euler(0, this.transform.localRotation.y, this.transform.localRotation.z);
            bodyPivot.transform.localRotation = Quaternion.Euler(0, bodyPivot.transform.localRotation.y, bodyPivot.transform.localRotation.z);
        }
    }

    //IEnumerator StartDash()
    //{
    //    Vector3 forward = camera.transform.forward;
    //    forward.y = 0;
    //    forward.Normalize();

    //    float starttime = Time.time;
    //    while (Time.time < starttime + DashTime)
    //    {
    //        DashForwardVelocity += decel * Time.deltaTime;
    //        DashForwardVelocity = Mathf.Clamp(DashForwardVelocity, 0, DashSpeed);
    //        con.Move(forward * DashForwardVelocity * Time.deltaTime);
    //        yield return null;
    //    }
    //}

    private void UpdateGrounded()
    {
        //Debug.Log("GROUNDED: " + isGround);
        //Debug.Log("SLIDE: " + isSlide);
        float valueToUse = 0;
        if (isSlide)
            valueToUse = bodyRadius;
        else
            valueToUse = bodyHeight;

        Ray laserRayCast = new Ray(body.transform.position, new Vector3(0, -1, 0));
        Debug.DrawRay(body.transform.position, new Vector3(0, -valueToUse - 0.01f, 0 ), Color.red);
        if (Physics.Raycast(laserRayCast, out RaycastHit hit, valueToUse + 0.01f))
        {
            isGround = true;
            return;
        }
        isGround = false;
    }

    //Camera functions
    public IEnumerator DoFOV(float endValue, float zoomSpeed) //adjust field of view
    {
        while (camera.fieldOfView != endValue)
        {
            camera.fieldOfView = Mathf.MoveTowards(camera.fieldOfView, endValue, zoomSpeed * Time.deltaTime);
            yield return null;
        }

        yield break;
    }

    public IEnumerator DoTiltZ(float endValue, float tiltSpeed) //adjust roll
    {
        while (roll != endValue)
        {
            roll = Mathf.MoveTowards(roll, endValue, tiltSpeed * Time.deltaTime);

            yield return null;
        }

        yield break;
    }
}

