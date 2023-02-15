using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Threading;

public class FPS : NetworkBehaviour
{
    // Start is called before the first frame update
    public CharacterController con;
    private Camera camera; // Main camera
    Vector3 moveVector;
    public bool isGround = false;
    private float pitch, yaw;
    private float CamSen;
    private float speed = 5f, DashSpeed = 30f, DashForwardVelocity, DashTime = 0.5f;
    private float decel;

    private Rigidbody rigidbody;

    //For double jump
    private bool doublejump = false;

    //For dash
    public float dashcooldown = 1.0f;
    public float dashDuration = 0.2f;
    float dashProgress = 0.2f;
    Vector3 storeDashDir;

    // gravity
    float gravity = -9.81f;
    Vector3 velocity;

    //For teleport
    bool canTeleport = true;

    // Drop kick
    bool dropKickActive = false;
    enum TeleportStates
    {
        NONE,
        TELEPORT_MARKER,
        TELEPORT_CHANNEL,
    };
    TeleportStates teleportState = TeleportStates.NONE;
    const float teleportDuration = 1.0f, teleportCooldown = 5.0f;
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
    void Start()
    {
        //Set dash progress to more than dash so dash isn't activated on start
        dashProgress = dashDuration + 1;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        pitch = yaw = 0f;
        CamSen = 220f;
        decel = -DashSpeed / DashTime;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        tpMarker = Instantiate(tpMarkerPrefab);
        tpMarkerMR = tpMarker.GetComponent<MeshRenderer>();
        tpMarkerMR.enabled = false;
        tpVerticalOffset = transform.localScale.y - tpMarker.transform.localScale.y; //do this whenever player rigidbody scale changes
        //currentEquipped = transform.parent.Find("Equipped");
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
       
        moveVector = (playerVerticalInput * forward) + (playerHorizontalInput * right); 
        moveVector.Normalize();
     
        // WASD movement
        if(isGround)
        {
            rigidbody.velocity = moveVector * speed;

        }
        else
        {
            rigidbody.AddForce(moveVector * speed * 5);
        }

        // Drop kick
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            dropKickActive = true;
        }

        if(dropKickActive)
        {
            if (isGround)
                dropKickActive = false;
            else
                rigidbody.velocity = new Vector3(0,-50,0);
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
        camera.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        //transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        //Position & gravity
        //con.Move(moveVector * speed * Time.deltaTime);
        //con.Move(this.GetComponent<Rigidbody>().velocity * Time.deltaTime);

        Jump();
        UpdateDash();
        //con.Move(velocity * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        
        camera.transform.position = transform.position;

        if (canTeleport) UpdateTeleport();


        //currentEquipped.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        //currentEquipped.transform.position = transform.position;
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
                rigidbody.AddForce(0, 500, 0);
                //velocity.y = Mathf.Sqrt(300 * Time.deltaTime * -2f * gravity);
                doublejump = true;
            }
            else if (doublejump)
            {
                rigidbody.AddForce(0, 500, 0);
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
            Vector3 bottomCenter = transform.position + Vector3.up * transform.localScale.y * 0.5f;
            Vector3 topCenter = bottomCenter + Vector3.up * transform.localScale.y;

            if (Physics.CapsuleCast(bottomCenter, topCenter, transform.localScale.x / 2, forward, out raycastHitForward, teleportDistance + transform.localScale.x * 0.25f))
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
                con.enabled = false;
                transform.position = tpMarker.transform.position + new Vector3(0, tpVerticalOffset, 0);
                con.enabled = true;
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
        dashProgress += Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // If no keyboard input, use camera direction
            if (moveVector.magnitude == 0)
            {
                Vector3 forward = camera.transform.forward;
                forward.y = 0;
                forward.Normalize();
                storeDashDir = forward;
                dashProgress = 0;

            }
            // Otherwise, dash towards movement input
            else
            {
                storeDashDir = moveVector;
                dashProgress = 0;
            }

            //DashForwardVelocity = DashSpeed;
            //dashstate = Dash.DASH;
            //dashProgress = dashcooldown;
        }
        if (dashProgress < dashDuration)
        {
            rigidbody.velocity = storeDashDir * speed * 3;
        }
        return;
        switch(dashstate)
        {
            case Dash.NONE:
                if (dashProgress <= 0)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        // If no keyboard input, use camera direction
                        if(moveVector.magnitude == 0)
                        {
                            Vector3 forward = camera.transform.forward;
                            forward.y = 0;
                            forward.Normalize();
                            rigidbody.velocity = forward * speed * 3;

                        }
                        // Otherwise, dash towards movement input
                        else
                        {
                            rigidbody.velocity = moveVector * speed * 3;
                        }

                        DashForwardVelocity = DashSpeed;
                        dashstate = Dash.DASH;
                        dashProgress = dashcooldown;
                    }
                }
                else
                {
                    dashProgress -= Time.deltaTime;
                }
                break;
            case Dash.DASH:
                //rigidbody.velocity.Set(0, 0, 0);
                //StartCoroutine(StartDash());
                dashstate = Dash.NONE;
                break;
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
        Ray laserRayCast = new Ray(transform.position, new Vector3(0,-1,0));
        if (Physics.Raycast(laserRayCast, out RaycastHit hit, 0.51f))
        {

            isGround = true;
            return;
        }
        isGround = false;
    }
}