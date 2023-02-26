using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class GrappleHook : NetworkBehaviour
{
    public Camera camera;
    [SerializeField] GameObject grappleBody;
    [SerializeField] GameObject hook;
    [SerializeField] GameObject line;
    private GameObject player;
    private Rigidbody grappledRigidBody;
    private FPS playerScript;
    private Rigidbody playerRigidBody;

    private NetworkVariable<bool> hookActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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

    public AudioSource AudioGrappleShoot;
    public AudioSource AudioGrappling;

    void Start()
    {
        player = transform.parent.parent.parent.gameObject; // this > left hand > equipped > player
        playerScript = player.GetComponent<FPS>();
        playerRigidBody = player.GetComponent<Rigidbody>();
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
   
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded : " + scene.name);
        if (scene.name == "RandallTestingScene" || scene.name == "Parallel_Pillars")
        {
            camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {        
        if (IsOwner)
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
            if (dotProduct > 0)
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

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (hookActive.Value)
                {
                    SetHookActive(false);
                }
                else if (playerScript.staminaAmount >= playerScript.staminaGrappleCost)
                {
                    AudioGrappleShoot.Play();
                    AudioGrappling.Play();

                    //Ray laserRayCast = new Ray(body.transform.position, 5 * (hook.transform.position - body.transform.position));
                    Ray laserRayCast = new Ray(camera.transform.position, camera.transform.forward);
                    Debug.DrawRay(camera.transform.position, 50 * (camera.transform.forward), Color.blue);
                    if (Physics.Raycast(laserRayCast, out RaycastHit hit, 100))
                    {
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
    }

    void LateUpdate()
    {
        if (hookActive.Value)
        {
            if (IsServer)
            {
                SetHookLineObjActiveServerRpc(true, true);

                SetHookLineRendererServerRpc(grappleBody.transform.position, hook.transform.position);
            }

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
            if (IsServer)
            {
                SetHookLineObjActiveServerRpc(false, false);
            }
        }
    }

    private void SetHookActive(bool active)
    {
        hookActive.Value = active;

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
            AudioGrappling.Stop();
            hook.transform.parent = this.transform;
            if (grappledRigidBody != null)
                grappledRigidBody.useGravity = true;

        }
    }

    [ServerRpc (RequireOwnership = false)]
    private void SetHookLineObjActiveServerRpc(bool hookActive, bool lineActive)
    {
        SetHookLineObjActiveClientRpc(hookActive, lineActive);
    }

    [ClientRpc]
    private void SetHookLineObjActiveClientRpc(bool hookActive, bool lineActive)
    {
        hook.SetActive(hookActive);
        line.SetActive(lineActive);
    }

    [ServerRpc (RequireOwnership = false)]
    private void SetHookLineRendererServerRpc(Vector3 grappleBodyPos, Vector3 hookPos)
    {
        SetHookLineRendererClientRpc(grappleBodyPos, hookPos);
    }
    [ClientRpc]
    private void SetHookLineRendererClientRpc(Vector3 grappleBodyPos, Vector3 hookPos)
    {
        LineRenderer grappleLine = line.GetComponent<LineRenderer>();
        grappleLine.positionCount = 2;
        grappleLine.SetPosition(0, grappleBodyPos);
        grappleLine.SetPosition(1, hookPos);
    }
}