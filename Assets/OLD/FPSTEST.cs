using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSTEST : MonoBehaviour
{
    public bool isGround = false;
    private float pitch, yaw;
    private float CamSen;
    private float speed = 5f;
    private Camera camera; // Main camera
    private Rigidbody rb;
    //For jump
    private bool doublejump = false, jumpispressed = false;
    float gravity = -9.81f;
    Vector3 MoveVector;
    const float dashcooldown = 1.0f, dashtimer = 0.25f;
    float dashProgress = 0.0f;


    float playerVerticalInput;
    float playerHorizontalInput;
    enum Dash
    {
        NONE,
        DASH
    }

    Dash dashstate = Dash.NONE;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        pitch = yaw = 0f;
        CamSen = 220f;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        transform.position = new Vector3(transform.position.x, 2.0f, transform.position.z);
    }

    private void Update()
    {
        playerVerticalInput = Input.GetAxis("Vertical"); // 1: W key , -1: S key, 0: no key input
        playerHorizontalInput = Input.GetAxis("Horizontal");

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        yaw += mouseX * CamSen * Time.deltaTime;
        pitch -= mouseY * CamSen * Time.deltaTime;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        MoveVector = (playerVerticalInput * forward) + (playerHorizontalInput * right);
        MoveVector.Normalize();

        //Rotation
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        float targetAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        camera.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        Vector3 temp = MoveVector * 300f * Time.deltaTime;
        //rb.velocity = new Vector3(temp.x, rb.velocity.y, temp.z);
        rb.MovePosition(rb.position + MoveVector * speed * Time.deltaTime);
        Jump();
        UpdateDash(forward);
        camera.transform.position = transform.position;

    }

    private void Jump()
    {
        RaycastHit raycasthit;
        Ray ray = new Ray(transform.position, -transform.up);

        if (Physics.Raycast(ray, out raycasthit, (GetComponent<CapsuleCollider>().height / 2) + 0.1f))
        {
            rb.drag = 10f;
            isGround = true;
        }
        else
        {
            rb.drag = 0;
            isGround = false;
        }

        if (!jumpispressed && Input.GetKey(KeyCode.Space))
        {
            if (isGround)
            {
                //rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(300 * Time.deltaTime * -2f * gravity), rb.velocity.z);
                rb.AddForce(Vector3.up * 300f * Time.deltaTime, ForceMode.Impulse);
                doublejump = true;
            }
            else if (doublejump)
            {
                //rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(300 * Time.deltaTime * -2f * gravity), rb.velocity.z);
                rb.AddForce(Vector3.up * 300f * Time.deltaTime, ForceMode.Impulse);
                doublejump = false;
            }
            jumpispressed = true;
        }
        else if (jumpispressed && !Input.GetKey(KeyCode.Space))
        {
            jumpispressed = false;
        }

    }


    private void UpdateDash(Vector3 forward)
    {
        switch (dashstate)
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
                StartCoroutine(StartDash(forward));
                dashstate = Dash.NONE;
                break;
        }
    }


    IEnumerator StartDash(Vector3 forward)
    {
        float dashforce = 300.0f;
        float starttime = Time.time;
        while (Time.time < starttime + dashtimer)
        {
            rb.AddForce(forward * dashforce * Time.deltaTime, ForceMode.Impulse);
            yield return null;
        }
    }
}
