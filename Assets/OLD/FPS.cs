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
    Vector3 MoveVector;
    public bool isGround = false;
    private float pitch, yaw;
    private float CamSen;
    private float speed = 5f, DashSpeed = 30f, DashForwardVelocity, DashTime = 0.5f;
    private float decel;

    //For double jump
    private bool doublejump = false;

    //For dash
    const float dashcooldown = 1.0f;
    float dashProgress = 0.0f;

    // gravity
    float gravity = -9.81f;
    Vector3 velocity;

    //For teleport
    bool canTeleport = true;
    enum TeleportStates
    {
        NONE,
        TELEPORT_MARKER,
        TELEPORT_CHANNEL,
    };
    TeleportStates teleportState = TeleportStates.NONE;
    const float teleportDuration = 1.0f, teleportCooldown = 5.0f;
    float teleportProgress = 0.0f, teleportCooldownTimer = 0.0f;
    float teleportDistance = 5.0f;
    float tpVerticalOffset = 0;
    [SerializeField] GameObject tpMarkerPrefab;
    GameObject tpMarker;
    MeshRenderer tpMarkerMR;

    private Transform currentEquipped;
    enum Dash
    {
        NONE,
        DASH
    }
    
    Dash dashstate = Dash.NONE;
    void Start()
    {

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
        currentEquipped = transform.parent.Find("Equipped");
        transform.position = new Vector3(transform.position.x, 2.0f, transform.position.z);
    }


    // Update is called once per frame
    void Update()
    {


        //if (!IsOwner) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float playerVerticalInput = Input.GetAxis("Vertical"); // 1: W key , -1: S key, 0: no key input
        float playerHorizontalInput = Input.GetAxis("Horizontal");

        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        yaw += mouseX * CamSen * Time.deltaTime;
        pitch -= mouseY * CamSen * Time.deltaTime;
        MoveVector = (playerVerticalInput * forward) + (playerHorizontalInput * right);
        MoveVector.Normalize();

        //Rotation
        pitch = Mathf.Clamp(pitch, -85f, 85f);
        float targetAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        camera.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        //Position & gravity
        con.Move(MoveVector * speed * Time.deltaTime);
        Jump();
        UpdateDash();
        con.Move(velocity * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        
        camera.transform.position = transform.position;

        if (canTeleport) UpdateTeleport();


        currentEquipped.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        currentEquipped.transform.position = transform.position;
        //Debug.Log(targetAngle);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (!IsOwner) return;
  
    }

    private void Jump()
    {
        RaycastHit raycasthit;
        Ray ray = new Ray(transform.position, -transform.up);

        if (Physics.Raycast(ray, out raycasthit, (GetComponent<CapsuleCollider>().height / 2) + 0.1f))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGround)
            {
                velocity.y = Mathf.Sqrt(300 * Time.deltaTime * -2f * gravity);
                doublejump = true;
            }
            else if (doublejump)
            {
                velocity.y = Mathf.Sqrt(300 * Time.deltaTime * -2f * gravity);
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
        if (Input.GetKeyDown(KeyCode.Alpha1) && teleportState < TeleportStates.TELEPORT_CHANNEL)
            StartTeleport();

        if (teleportState == TeleportStates.TELEPORT_MARKER)
        {
            Vector3 forward = camera.transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 tpMarkerPos = transform.position + forward * teleportDistance;

            //Add raycasts down
            tpMarkerPos.y = 10.0f; //Start from high enough
            RaycastHit raycasthit;
            Ray ray = new Ray(tpMarkerPos, -transform.up);

            if (Physics.Raycast(ray, out raycasthit, 11.0f))
            {
                tpMarker.transform.position = raycasthit.point + new Vector3(0, tpMarker.transform.localScale.y, 0);
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
            }
        }
    }

    private void UpdateDash()
    {
        switch(dashstate)
        {
            case Dash.NONE:
                if (dashProgress <= 0)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
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
                StartCoroutine(StartDash());
                dashstate = Dash.NONE;
                break;
       }      
    }

    IEnumerator StartDash()
    {
        Vector3 forward = camera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        float starttime = Time.time;
        while (Time.time < starttime + DashTime)
        {
            DashForwardVelocity += decel * Time.deltaTime;
            DashForwardVelocity = Mathf.Clamp(DashForwardVelocity, 0, DashSpeed);
            con.Move(forward * DashForwardVelocity * Time.deltaTime);
            yield return null;
        }
    }
}
