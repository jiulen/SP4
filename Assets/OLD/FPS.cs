using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class FPS : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera camera;
    private Rigidbody rb;
    public bool isGround = false;
    private float pitch, yaw;
    private float CamSen;
    private float speed = 5f;
    private bool doublejump = false, jumpispressed = false;
    private Vector3 offset;

    void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        pitch = yaw = 0f;
        CamSen = 220f;
        rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() // mouse sen smooth
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        yaw += mouseX * CamSen * Time.deltaTime;
        pitch -= mouseY * CamSen * Time.deltaTime;
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        float playerVerticalInput = Input.GetAxis("Vertical"); // 1: W key , -1: S key, 0: no key input
        float playerHorizontalInput = Input.GetAxis("Horizontal");

        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 MoveVector = (playerVerticalInput * forward) + (playerHorizontalInput * right);
        pitch = Mathf.Clamp(pitch, -85f, 85f);


        float targetAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;


        camera.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        rb.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        Jump();
        rb.MovePosition(rb.position + MoveVector * speed * Time.deltaTime);
        camera.transform.position = rb.position;


        //Debug.Log(targetAngle);
    }

    private void Jump()
    {
        RaycastHit raycasthit;
        Ray ray = new Ray(rb.position, -transform.up);

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
                rb.velocity = new Vector3(0, 300 * Time.deltaTime, 0);
                doublejump = true;
            }
            else if (doublejump)
            {
                rb.velocity = new Vector3(0, 300 * Time.deltaTime, 0);
                doublejump = false;
            }
            jumpispressed = true;
        }
        else if (jumpispressed && !Input.GetKey(KeyCode.Space))
        {
            jumpispressed = false;
        }
    }

}
