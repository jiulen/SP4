using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    enum TeleportStates
    {
        NONE,
        TELEPORT_MARKER,
        TELEPORT_CHANNEL,
    };
    TeleportStates teleportState = TeleportStates.NONE;
    const float teleportDuration = 1.0f, teleportCooldown = 1.0f;
    float teleportProgress = 0.0f, teleportCooldownTimer = teleportCooldown;
    float teleportDistance = 10.0f;
    [SerializeField] GameObject tpMarkerPrefab;
    GameObject tpMarker;
    LayerMask terrain;

    Transform playerHead;
    Transform playerBody;
    FPS playerFPSScript;

    // Start is called before the first frame update
    void Start()
    {
        tpMarker = Instantiate(tpMarkerPrefab);
        playerFPSScript = GetComponent<FPS>();
        playerHead = playerFPSScript.head.transform;
        playerBody = playerFPSScript.body.transform;

        terrain = 1 << LayerMask.NameToLayer("Terrain");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerFPSScript.canTeleport)
            UpdateTeleport();
    }

    private void UpdateTeleport()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            teleportState = TeleportStates.NONE;
            tpMarker.SetActive(false);
        }
        
        switch (teleportState)
        {
            case TeleportStates.NONE:
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1) && teleportCooldownTimer >= teleportCooldown)
                        teleportState = TeleportStates.TELEPORT_MARKER;

                    teleportCooldownTimer += Time.deltaTime;

                    break;
                }
            case TeleportStates.TELEPORT_MARKER:
                {
                    if (Physics.Raycast(playerHead.position, playerHead.forward, out RaycastHit raycastForwardHit, teleportDistance, terrain))
                    {
                        if (Physics.Raycast(raycastForwardHit.point + new Vector3(0, 2.5f, 0), Vector3.down, out RaycastHit raycastDownHit, 5f, terrain))
                        {
                            if (raycastForwardHit.point == raycastDownHit.point)
                            {
                                tpMarker.transform.position = raycastDownHit.point + new Vector3(0, tpMarker.transform.localScale.y * 0.5f, 0);
                            }
                            else
                            {
                                tpMarker.transform.position = raycastDownHit.point + new Vector3(0, tpMarker.transform.localScale.y * 0.5f, 0) + playerHead.forward * playerBody.localScale.x * 0.5f;
                            }
                            
                            tpMarker.SetActive(true);
                            if (Input.GetKeyDown(KeyCode.Alpha1))
                            {
                                teleportProgress = 0.0f;
                                teleportState = TeleportStates.TELEPORT_CHANNEL;
                            }
                        }
                        else
                            tpMarker.SetActive(false);
                    }
                    else
                        tpMarker.SetActive(false);

                    break;
                }
            case TeleportStates.TELEPORT_CHANNEL:
                {
                    if (teleportProgress < teleportDuration)
                    {
                        //Channel teleport
                        teleportProgress += Time.deltaTime;
                    }
                    else
                    {
                        //Do teleport
                        transform.position = tpMarker.transform.position + new Vector3(0, 1f, 0);

                        teleportState = TeleportStates.NONE;
                        teleportCooldownTimer = 0;
                        tpMarker.SetActive(false);
                    }

                    break;
                }
        }
    }
}
