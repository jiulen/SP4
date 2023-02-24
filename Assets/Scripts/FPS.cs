using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Threading;
using UnityEngine.SceneManagement;

// Player script for movement and abilities
public class FPS : NetworkBehaviour
{
    // Debug
    public bool debugBelongsToPlayer = false;

    // General
    public GameObject body;
    public GameObject head;
    [SerializeField] GameObject bodyPivot;
    [SerializeField] GameObject headPivot;
    [SerializeField] MeshRenderer visorMR;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;
    public float airMovementMultiplier = 2.5f;
    public float runSpeed = 10f;
    CapsuleCollider capsuleCollider;
    public Camera camera; // Main camera
    Vector3 moveVector;
    private float pitch, yaw, roll;
    private float CamSen;
    private Rigidbody rigidbody;
    public bool cameraMoveEnabled = true;


    // Stamina
    public float staminaJumpCost = 0.5f;
    public float staminaDashCost = 1.0f;
    public float staminaGrappleCost = 1.0f;
    public float staminaTeleportCost = 2.0f;
    public float staminaWallrunRate = 1.0f;
    public float staminaSlideRate = 1.0f;

    public float staminaAmount = 0;
    public int staminaMax = 3;

    public float staminaRefillRate = 1.0f;

    // Grounded check
    private float headHeight = 1;
    private float bodyHeight = 1;
    private float bodyRadius = 1;
    private double airTimer = 0;
    private bool isGround = false;


    // Jump
    public float jumpForce = 250;

    // Dash
    public float dashSpeed = 5;
    public float dashDuration = 0.2f;
    float dashProgress = 0.2f;
    public bool candash = true;
    public int dashNum = 3;
    private Vector3 storeDashDir;

    // Slide
    public float slideMultiplier = 2;
    private bool isSlide = false;
    private Vector3 storeSlideVelocity;

    // Grapple
    public bool isGrapple = false;

    //// Gravity
    //float gravity = -9.81f;
    //Vector3 velocity;

    // Drop kick
    bool dropKickActive = false;

    // Teleport
    public bool canTeleport = true;

    private Transform currentEquipped;

    //Dash
    private bool forcedash = false;
    public void SetForcedash(bool force)
    {
        forcedash = force;
    }

    enum Dash
    {
        NONE,
        DASH
    }

    Dash dashstate = Dash.NONE;

    //Dash camera effects
    Coroutine zoomInCoroutine, zoomOutCoroutine;

    //Wallrunning
    public bool canWallrun = true;
    public bool isWallrunning = false;

    [SerializeField] GameObject[] weaponPrefabList;
    [SerializeField] GameObject[] hatPrefabList;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        capsuleCollider = head.GetComponent<CapsuleCollider>();

        headHeight = head.transform.position.y - body.transform.position.y + (body.GetComponent<CapsuleCollider>().height * head.transform.localScale.y * body.transform.localScale.y) / 2;

        // bodyHeight refers to its relative y position, not the height of the collider
        bodyHeight = (body.GetComponent<CapsuleCollider>().height * head.transform.localScale.y * body.transform.localScale.y) / 2;
        bodyRadius = (body.GetComponent<CapsuleCollider>().radius * head.transform.localScale.y * body.transform.localScale.y);
        //Set dash progress to more than dash so dash isn't activated on start
        dashProgress = dashDuration + 1;
        pitch = yaw = roll = 0f;
        CamSen = 220f;

        currentEquipped = transform.Find("Equipped");
        transform.position = new Vector3(transform.position.x, 2.0f, transform.position.z);
        rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.velocity.Set(0, 0, 0);
    }

    public override void OnNetworkSpawn() //must do check for IsOwner in OnNetworkSpawn (IsOwner only updates after awake)
    {
        base.OnNetworkSpawn();

        IsOwnerStartCheck();
    }

    private void IsOwnerStartCheck()
    {
        if (!IsOwner && !debugBelongsToPlayer) return;

        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        body.GetComponent<MeshRenderer>().enabled = false;
        visorMR.enabled = false;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded : " + scene.name);
        if (scene.name == "RandallTestingScene")
        {
            Init();
            IsOwnerStartCheck();
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            AddWeaponServerRpc("Shotgun");

        // Reset position and velocity if player goes out of bounds for debugging
        if (transform.position.magnitude > 100 || transform.position.y <= -20)
        {
            transform.position = new Vector3(0, 10, 0);
            rigidbody.velocity = new Vector3(0, 0, 0);
        }
        UpdateGrounded();

        if (!IsOwner && !debugBelongsToPlayer) return;
        Debug.DrawRay(camera.transform.position, 100 * camera.transform.forward, Color.black);

        

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float playerVerticalInput = Input.GetAxisRaw("Vertical"); // 1: W key , -1: S key, 0: no key input
        float playerHorizontalInput = Input.GetAxisRaw("Horizontal");

        if (cameraMoveEnabled)
        {
            yaw += mouseX * CamSen * Time.deltaTime;
            pitch -= mouseY * CamSen * Time.deltaTime;
            camera.transform.rotation = Quaternion.Euler(pitch, yaw, roll); 
        }

        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        if (isWallrunning)
        {
        }
        else
        {
            //Disable movement while wallrunning

            moveVector = (playerVerticalInput * forward) + (playerHorizontalInput * right);
            moveVector.Normalize();

            // WASD movement
            if (isGrapple)
            {
                Vector3 moveVectorWithY;
                moveVectorWithY = (playerVerticalInput * camera.transform.forward) + (playerHorizontalInput * camera.transform.right);
                moveVectorWithY.Normalize();
                rigidbody.AddForce(moveVectorWithY * airMovementMultiplier);
                rigidbody.useGravity = false;
            }
            else if (isGround)
            {
                rigidbody.useGravity = true;

                //moveVector = (playerVerticalInput * forward) + (playerHorizontalInput * right);
                //moveVector.Normalize();
                Vector3 newDirection = moveVector * runSpeed;
                newDirection.y = rigidbody.velocity.y;
                rigidbody.velocity = newDirection;
                //if (rigidbody.velocity.magnitude >= runSpeed)
                //{
                //    rigidbody.velocity = rigidbody.velocity.normalized * runSpeed;
                //}
            }
            else
            {
                rigidbody.useGravity = true;

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
                        if (moveVector.magnitude == 0)
                        {
                            storeSlideVelocity = camera.transform.forward.normalized * runSpeed * slideMultiplier;

                        }
                        else
                        {
                            storeSlideVelocity = moveVector * runSpeed * slideMultiplier;
                        }
                    }
                }
                else
                {
                    if(!isSlide)
                       dropKickActive = true;
                }
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
            //    rigidbody.AddForce(-rigidbody.velocity.normalized * runSpeed);

            //}
            //else
            //{
            //    Debug.Log("ruh roh");
            //    rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
            //    rigidbody.AddForce(moveVector * 100);
            //}
            //this.GetComponent<Rigidbody>().velocity = moveVector;

            Jump();
            UpdateDash();
            UpdateSlide();
            //velocity.y += gravity * Time.deltaTime;
        }

        //Rotation
        pitch = Mathf.Clamp(pitch, -89f, 89f);
        float targetAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, targetAngle, roll);
        head.transform.localRotation = Quaternion.Euler(pitch, 0, 0); //rotate head to face same dir as camera
        headPivot.transform.localRotation = Quaternion.Euler(-pitch, 0, 0); //rotate body to reverse head rotation

        camera.transform.position = head.transform.position;
        //Sniper sniper = transform.GetComponent<Sniper>();
        //if (sniper != null && sniper.Scoped.enabled)
        //{
        //    camera.transform.position += camera.transform.up * (Mathf.Sin(sniper.stablizeElasped * 2) / 2) * 0.4f + camera.transform.right * Mathf.Cos(sniper.stablizeElasped) * 0.4f;
        //}

        currentEquipped.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        currentEquipped.transform.position = head.transform.position;
        //Debug.Log(targetAngle);
        if (isSlide || isWallrunning)
        {

        }
        else
        {
            float refillAmount = staminaRefillRate;
            if (airTimer >= 0.75f)
                refillAmount = staminaRefillRate * 0.25f;

            staminaAmount += refillAmount * Time.deltaTime;
            if (staminaAmount > staminaMax)
            {
                staminaAmount = staminaMax;
            }
        }


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (!IsOwner) return;

        // Apply air resistance
        if(!isGround)
        {
            rigidbody.AddForce(-rigidbody.velocity/1.5f);
        }

        // Limit speed
        //Vector3 velocityWithoutY = rigidbody.velocity;
        //velocityWithoutY.y = 0;
        //if ( rigidbody.velocity.magnitude >= runSpeed)
        //{
        //    velocityWithoutY = velocityWithoutY.normalized * runSpeed;
        //    velocityWithoutY.y = rigidbody.velocity.y;
        //    //rigidbody.velocity = velocityWithoutY.normalized * 5;
        //    rigidbody.velocity = velocityWithoutY;
        //}
    }

    private void Jump()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGround)
            {
                if(isGrapple)
                    rigidbody.AddForce(0, jumpForce * 2.5f, 0);
                else
                    rigidbody.AddForce(0, jumpForce, 0);
                //velocity.y = Mathf.Sqrt(300 * Time.deltaTime * -2f * gravity);
                
                if (isSlide)
                {
                    //this.transform.position = new Vector3(this.transform.position.x, headHeight, this.transform.position.z);
                    rigidbody.AddForce(0, jumpForce, 0);

                    //rigidbody.AddForce(rigidbody.velocity);

                }
            }
            else if(staminaAmount >= staminaJumpCost)
            {
                rigidbody.AddForce(0, jumpForce, 0);
                staminaAmount -= staminaJumpCost;
               
            }
        }

    }

    private void UpdateDash()
    {
        //Debug.Log(dashMetre);
        //Debug.Log(dashMetreMax);
        //Debug.Log(dashNum);
        dashProgress += Time.deltaTime;

       

        if ((Input.GetKeyDown(KeyCode.LeftShift) || forcedash) && staminaAmount >= staminaDashCost && candash)
        {
            // If no keyboard input, use camera direction
            if (moveVector.magnitude == 0 || forcedash)
            {
                Vector3 forward = camera.transform.forward;
                forward.y = 0;
                forward.Normalize();
                storeDashDir = forward;
                dashProgress = 0;
                staminaAmount -= staminaDashCost;
                forcedash = false;
            }
            // Otherwise, dash towards movement input
            else
            {
                storeDashDir = moveVector;
                dashProgress = 0;
                staminaAmount -= staminaDashCost;
            }
        }
        if (dashProgress < dashDuration)
        {
            // If the player is falling, cancel their vertical velocity
            if (rigidbody.velocity.y < 0)
                rigidbody.velocity = storeDashDir * dashSpeed * 3;
            // If the player is jumping, maintain that velocity
            else
            {
                Vector3 newVelocity = storeDashDir * dashSpeed * 3;
                newVelocity.y = rigidbody.velocity.y;
                rigidbody.velocity = newVelocity;
            }


            if (zoomInCoroutine != null) StopCoroutine(zoomInCoroutine);
            if (zoomOutCoroutine != null) StopCoroutine(zoomOutCoroutine);

            if (candash)
            {
                zoomOutCoroutine = StartCoroutine(DoFOV(75f, 200f));
            }
        }
        else
        {

            if (zoomInCoroutine != null) StopCoroutine(zoomInCoroutine);
            if (zoomOutCoroutine != null) StopCoroutine(zoomOutCoroutine);

            if (candash)
            {
                zoomInCoroutine = StartCoroutine(DoFOV(60f, 150f));
            }
        }

        return;
    }

    private void UpdateSlide()
    {
        if (isSlide)
        {

            if (airTimer >= 0.5)
            {
                isSlide = false;
                return;
            }

            staminaAmount -= staminaSlideRate * Time.deltaTime;
            if (staminaAmount < 0)
            {
                staminaAmount = 0;
                isSlide = false;
                return;
            }
            // Change the vel.y so player maintains vertical momentum
            Vector3 newVel = storeSlideVelocity;
            newVel.y = rigidbody.velocity.y;
            rigidbody.velocity = newVel;
            //rigidbody.AddForce(-storeVelocityForSlide.normalized );
            float yAngle = Mathf.Atan2(storeSlideVelocity.x,storeSlideVelocity.z) * Mathf.Rad2Deg; // Convert the vector to an angle in degrees
            if (yAngle < 0)
            {
                yAngle += 360;
            }

            head.transform.LookAt(newVel);
            head.transform.localRotation = Quaternion.Euler( -45, yAngle, head.transform.localRotation.z);
            bodyPivot.transform.localRotation = Quaternion.Euler(-45, bodyPivot.transform.localRotation.y, bodyPivot.transform.localRotation.z);
        }
        else
        {
            head.transform.localRotation = Quaternion.Euler(0, head.transform.localRotation.y, head.transform.localRotation.z);
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
        valueToUse = bodyHeight;
        Ray laserRayCast = new Ray(body.transform.position, new Vector3(0, -1, 0));
        Debug.DrawRay(this.transform.position, new Vector3(0, -valueToUse - 0.01f, 0 ), Color.red);
        if (Physics.Raycast(laserRayCast, out RaycastHit hit, valueToUse + 0.01f))
        {
            isGround = true;
            airTimer = 0;
            return;
        }
        isGround = false;
        airTimer += Time.deltaTime;
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

    public bool GetIsGrounded()
    {
        return isGround;
    }

    [ServerRpc]
    public void AddWeaponServerRpc(string weaponName) //For adding general weapons (character specific weapons added separately)
    {
        // List of weapon names:
        // (General)
        // DickScat
        // Grenade
        // RPG
        // Shotgun
        // Sniper
        // Staff
        // Sword

        GameObject weapon = null;

        switch (weaponName)
        {
            case "DickScat":
                weapon = Instantiate(weaponPrefabList[3]);
                break;
            case "Grenade":
                weapon = Instantiate(weaponPrefabList[4]);
                break;
            case "RPG":
                weapon = Instantiate(weaponPrefabList[5]);
                break;
            case "Shotgun":
                weapon = Instantiate(weaponPrefabList[6]);
                break;
            case "Sniper":
                weapon = Instantiate(weaponPrefabList[7]);
                break;
            case "Staff":
                weapon = Instantiate(weaponPrefabList[8]);
                break;
            case "Sword":
                weapon = Instantiate(weaponPrefabList[9]);
                break;
        }

        if (weapon)
        {
            weapon.GetComponent<NetworkObject>().Spawn(true);
            weapon.GetComponent<NetworkObject>().TrySetParent(rightHand);
            //weapon.transform.SetParent(rightHand);
        }
    }

    [ServerRpc]
    public void SetCharacterServerRpc(string charName)
    {
        // List of character names:
        // Rhino
        // Angler
        // Winton

        GameObject weapon = null, hat = null;

        switch (charName)
        {
            case "Rhino":
                weapon = Instantiate(weaponPrefabList[0]);
                hat = Instantiate(hatPrefabList[0]);
                break;
            case "Angler":
                weapon = Instantiate(weaponPrefabList[1]);
                hat = Instantiate(hatPrefabList[1]);
                break;
            case "Winton":
                weapon = Instantiate(weaponPrefabList[2]);
                hat = Instantiate(hatPrefabList[2]);
                break;
        }

        if (weapon && hat)
        {
            weapon.GetComponent<NetworkObject>().Spawn(true);
            hat.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}
