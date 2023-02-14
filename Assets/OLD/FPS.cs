using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class FPS : MonoBehaviour
{
    // Start is called before the first frame update
    public CharacterController con;
    private Camera camera; // Main camera
   // private Rigidbody rb;
    public bool isGround = false;
    private float pitch, yaw;
    private float CamSen;
    private float speed = 5f;
    private Vector3 offset;

    //For jump
    private bool doublejump = false, jumpispressed = false;

    //For teleport
    bool teleporting = false;
    const float teleportDuration = 1.0f;
    const float dashcooldown = 1.0f;
    float teleportProgress = 0.0f;
    float dashProgress = 0.0f;
    Vector3 teleportLocation = new Vector3();
    Vector3 MoveVector;


    float gravity = -9.81f;
    Vector3 velocity;
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
        //rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        yaw += mouseX * CamSen * Time.deltaTime;
        pitch -= mouseY * CamSen * Time.deltaTime;

        float playerVerticalInput = Input.GetAxis("Vertical"); // 1: W key , -1: S key, 0: no key input
        float playerHorizontalInput = Input.GetAxis("Horizontal");

        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        MoveVector = (playerVerticalInput * forward) + (playerHorizontalInput * right);
        MoveVector.Normalize();

        //Rotation
        pitch = Mathf.Clamp(pitch, -85f, 85f);
        float targetAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        camera.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        //Position
        // rb.MovePosition(rb.position + MoveVector * speed * Time.deltaTime);

        con.Move(MoveVector * speed * Time.deltaTime);
        Jump();

        con.Move(velocity * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        camera.transform.position = transform.position;


        //Debug.Log(targetAngle);
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

        if (!jumpispressed && Input.GetKey(KeyCode.Space))
        {
            if (isGround)
            {
                //rb.velocity = new Vector3(0, 300 * Time.deltaTime, 0);
                velocity.y = Mathf.Sqrt(300 * Time.deltaTime * -2f * gravity);
                doublejump = true;
            }
            else if (doublejump)
            {
                //rb.velocity = new Vector3(0, 300 * Time.deltaTime, 0);
                velocity.y = Mathf.Sqrt(300 * Time.deltaTime * -2f * gravity);
                doublejump = false;
            }
            jumpispressed = true;
        }
        else if (jumpispressed && !Input.GetKey(KeyCode.Space))
        {
            jumpispressed = false;
        }

    }

    private void StartTeleport()
    {
        teleportProgress = 0.0f;
        teleporting = true;

        Vector3 forward = camera.transform.forward;
        forward.y = 0;
        forward.Normalize();
        float teleportDistance = 10.0f;

        //teleportLocation = rb.position + forward * teleportDistance;
    }


    private void UpdateDash(Vector3 forward)
    {
        Vector3 temp = forward;
        temp.y = 0;
        temp.Normalize();
        switch(dashstate)
        {
            case Dash.NONE:
                if (dashProgress <= 0)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
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
                float dashforce = 500.0f;
                //rb.AddForce(temp * dashforce * Time.deltaTime);
                dashstate = Dash.NONE;
                break;
        }
    }
}
