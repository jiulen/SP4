using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Unity.Netcode;

// Player script for UI, playerstats
public class PlayerEntity : EntityBase
{
    private FPS FPSScript;
    public GameObject[] equippedWeaponList = new GameObject[3];
    public GameObject activeWeapon;
    public GameObject previousWeapon;
    private float respawncountdown = 5, currentrespawnelaspe;
    private Image uiCurrentWeaponIcon;
    private Image cameraEffectInjured, cameraEffectBlood;
    private GameObject uiWeaponWheelCanvas;
    private GameObject uiPlayerStatsCanvas;
    private GameObject uiDeathCanvas;
    private GameObject uiStaminaCanvas;
    private GameObject uiKillFeedCanvas;
    private GameObject crosshairCanvas;
    private GameObject cameraEffectsCanvas;
    private GameObject playerCanvasParent;
    private GameObject dashEffectCanvas;
    private ParticleSystem dashParticleSystem;
    public GameObject PlayerCanvasParentPF;
    //public GameObject UIWeaponWheelPF;
    //public GameObject PlayerStatsCanvasPF;
    public GameObject StaminaCanvasPF;
    //public GameObject CrosshairCanvasPF;
    //public GameObject DeathCanvasPF;
    //public GameObject CameraEffectsCanvasPF;
    public GameObject KillFeedPrefab;
    private static List<GameObject> KillPrefabList = new List<GameObject>();

    void Awake()
    {
        FPSScript = this.GetComponent<FPS>();
        base.Start();
        //InitialiseUI();
        uiKillFeedCanvas = GameObject.Find("Canvas/KillerFeedUI");
    }

    void InitialiseUI()
    {
        if (!FPSScript.IsOwner && !FPSScript.debugBelongsToPlayer) return;

        playerCanvasParent = Instantiate(PlayerCanvasParentPF, this.transform);
        uiWeaponWheelCanvas = playerCanvasParent.transform.Find("Weapon Wheel Canvas V2").gameObject;
        uiDeathCanvas = playerCanvasParent.transform.Find("Death Canvas").gameObject;
        uiPlayerStatsCanvas = playerCanvasParent.transform.Find("Player Stats Canvas").gameObject;
        uiCurrentWeaponIcon = uiPlayerStatsCanvas.transform.Find("CurrentWeaponImage").GetComponent<Image>();
        cameraEffectsCanvas = playerCanvasParent.transform.Find("Camera Effects Canvas").gameObject;
        cameraEffectInjured = cameraEffectsCanvas.transform.Find("Injured").GetComponent<Image>();
        cameraEffectBlood = cameraEffectsCanvas.transform.Find("Blood").GetComponent<Image>();

        uiStaminaCanvas = playerCanvasParent.transform.Find("Stamina Canvas").gameObject;
        uiStaminaCanvas.GetComponent<StaminaUI>().InitBars();

        crosshairCanvas = playerCanvasParent.transform.Find("Custom Crosshair Canvas").gameObject;
        crosshairCanvas.GetComponent<CustomCrosshair>().doUpdate = true;

        dashEffectCanvas = playerCanvasParent.transform.Find("Dash Canvas").gameObject;
        dashParticleSystem = dashEffectCanvas.transform.GetChild(0).GetComponent<ParticleSystem>();

        // I am too lazy to make sure that the start functions are called in the right order so I just grab the camera from the top of the hierarchy
        dashEffectCanvas.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

    }

    void Start()
    {
        InitialiseUI();
        currentrespawnelaspe = respawncountdown;
    }

    private void OnPointerEnterDelegate(BaseEventData eventData, Button items, int idx)
    {
        HoverListener hoverListener = items.GetComponent<HoverListener>();
        if (hoverListener != null)
        {
            hoverListener.idx = idx;
            hoverListener.OnPointerEnter((PointerEventData)eventData);
        }
    }
    public void OnPointerExitDelegate(BaseEventData eventData, Button items, int idx)
    {
        HoverListener hoverListener = items.GetComponent<HoverListener>();
        if (hoverListener != null)
        {
            hoverListener.idx = idx;
            hoverListener.OnPointerExit((PointerEventData)eventData);
        }
    }

    [ClientRpc]
    private void UpdateKillFeedClientRpc(ulong networkObject, ulong networkObject2, string name1, string name2)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObject, out NetworkObject networkObject1))
        {
            networkObject1.gameObject.transform.Find("PlayerKill").GetComponent<Text>().text = name1;
            networkObject1.gameObject.transform.Find("PlayerDeath").GetComponent<Text>().text = name2;
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObject2, out NetworkObject networkObject22))
            {
                networkObject1.gameObject.transform.Find("KillerWeapon").GetComponent<Image>().sprite = networkObject22.gameObject.GetComponent<WeaponBase>().WeaponIcon;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnKillFeedServerRpc(ulong playerkills, ulong playerdeath, ulong typeofWeapon)
    {
        GameObject t1 = null, t2 = null;
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerkills, out NetworkObject networkObject1))
            t1 = networkObject1.gameObject;
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerdeath, out NetworkObject networkObject2))
            t2 = networkObject2.gameObject;

        GameObject spawnFeed = Instantiate(KillFeedPrefab, uiKillFeedCanvas.transform);
        spawnFeed.GetComponent<NetworkObject>().Spawn();
        UpdateKillFeedClientRpc(spawnFeed.GetComponent<NetworkObject>().NetworkObjectId, typeofWeapon, t1.name, t2.name);
        spawnFeed.GetComponent<NetworkObject>().TrySetParent(uiKillFeedCanvas);
        KillPrefabList.Insert(0, spawnFeed);

        if (KillPrefabList.Count >= 5)
        {
            Destroy(KillPrefabList[KillPrefabList.Count - 1].gameObject);
            KillPrefabList.Remove(KillPrefabList[KillPrefabList.Count - 1]);
        }


        int i = 0;
        foreach (GameObject killfeed in KillPrefabList)
        {
            Vector2 killfeedpos = KillPrefabList[0].GetComponent<RectTransform>().anchoredPosition;
            killfeed.GetComponent<RectTransform>().anchoredPosition = new Vector2(killfeedpos.x, killfeedpos.y + (i * 25));
            killfeed.transform.parent = uiKillFeedCanvas.transform;
            i++;
        }


    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        //Debug.Log(Health);
        if (!FPSScript.IsOwner && !FPSScript.debugBelongsToPlayer) return;

        if (Health <= 0)
        {
            uiDeathCanvas.SetActive(true);
            UpdateDead(GetComponent<NetworkObject>().NetworkObjectId);
        }
        else
        {
            uiDeathCanvas.SetActive(false);
        }

        //Weapon UI icon
        if (activeWeapon != null)
           uiCurrentWeaponIcon.sprite = activeWeapon.GetComponent<WeaponBase>().WeaponIcon;


        Color currAlpha = cameraEffectInjured.color;
        currAlpha.a = 1 - (Health / MaxHealth);
        cameraEffectInjured.color = currAlpha;

        uiStaminaCanvas.GetComponent<StaminaUI>().UpdateStamina(FPSScript.staminaAmount);

    }

    [ClientRpc]
    private void SpawnDamageIndicatorClientRpc(Vector3 dir, ClientRpcParams clientRpcParams = default)
    {
        GameObject dmgImg = FetchDamageIndicator();
        dmgImg.GetComponent<DamageIndicator>().SetSourcePos(dir);
        cameraEffectBlood.GetComponent<BloodEffects>().ResetStartDuration();
    }

    private GameObject FetchDamageIndicator()
    {
        for (int i = 0; i < cameraEffectsCanvas.transform.GetChild(0).childCount; i++)
        {
            Transform cSolidObj = cameraEffectsCanvas.transform.GetChild(0).GetChild(i);
            if (cSolidObj.GetComponent<Image>().enabled)
            {
                continue;
            }
            cSolidObj.GetComponent<Image>().enabled = true;
            return cSolidObj.gameObject;
        }

        int prevSize = cameraEffectsCanvas.transform.GetChild(0).childCount;
        cameraEffectsCanvas.transform.GetChild(0).GetChild(prevSize).GetComponent<Image>().enabled = true;
        return cameraEffectsCanvas.transform.GetChild(0).GetChild(prevSize).gameObject;
    }

    [ServerRpc(RequireOwnership = false)]
    private void testServerRpc(float hp, Vector3 dir, ulong source, ulong weaponUsed)
    {
        testClientRpc(hp, dir, source, weaponUsed);
    }

    public override void TakeDamage(float hp, Vector3 dir, GameObject source, GameObject weaponUsed)
    {
        testServerRpc(hp, dir, source.GetComponent<NetworkObject>().OwnerClientId, weaponUsed.GetComponent<NetworkObject>().OwnerClientId);
    }
    [ClientRpc]
    private void testClientRpc(float hp, Vector3 dir, ulong source, ulong weaponUsed)
    {
        GameObject t1 = null;
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(source, out NetworkObject networkObject1))
            t1 = networkObject1.gameObject;

        SpawnDamageIndicatorClientRpc(dir, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { gameObject.GetComponent<NetworkBehaviour>().OwnerClientId }
            }
        });

        SetLastTouch(t1);
        SetHealth(GetHealth() - hp);

        if (Health <= 0)
        {
            SpawnKillFeedServerRpc(source, this.gameObject.GetComponent<NetworkObject>().NetworkObjectId, weaponUsed);
        }
    }

    public void UpdateDead(ulong lastTouchSource)
    {
        uiDeathCanvas.transform.Find("DeathTimerTxt").GetComponent<Text>().text = ((int)currentrespawnelaspe).ToString();

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(lastTouchSource, out NetworkObject networkObject2))
            uiDeathCanvas.transform.Find("DeathTxt").GetComponent<Text>().text = "You are killed by " + networkObject2.gameObject.GetComponent<NetworkObject>().name;

        if ((int)currentrespawnelaspe <= 0)
        {
            ResetPlayerServerRpc();
        }
        else
        {
            FPSScript.enabled = false;
            transform.Find("PlayerPivot").gameObject.SetActive(false);
            transform.GetComponent<Teleport>().enabled = false;
            transform.GetComponent<PickupCollectibles>().enabled = false;
            currentrespawnelaspe -= Time.deltaTime;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResetPlayerServerRpc()
    {
        NotifyResetPlayerClientRpc();
    }

    [ClientRpc]
    private void NotifyResetPlayerClientRpc()
    {
        if (Health <= 0)
        {
            FPSScript.enabled = true;
            transform.GetComponent<Teleport>().enabled = true;
            transform.GetComponent<PickupCollectibles>().enabled = true;
            transform.Find("PlayerPivot").gameObject.SetActive(true);
            SetHealth(MaxHealth);
            currentrespawnelaspe = respawncountdown;
        }
    }

    public string GetWeaponName(int idx)
    {
        return equippedWeaponList[idx].name;
    }

    public GameObject GetCrosshairCanvas()
    {
        return crosshairCanvas;
    }

    public GameObject GetWeaponWheelCanvas()
    {
        return uiWeaponWheelCanvas;
    }
    public void SetActiveWeapon(int i)
    {
        //if (activeWeapon != null)
            activeWeapon.SetActive(false);

        // Swap to previous weapon
        if(i == -1)
        {
            GameObject tempReference = previousWeapon;
            previousWeapon.SetActive(true);

            previousWeapon = activeWeapon;
            activeWeapon = tempReference;
        }
        else
        {
            equippedWeaponList[i].SetActive(true);
            previousWeapon = activeWeapon;
            activeWeapon = equippedWeaponList[i];
        }

    }

    public void StartDashEffect()
    {
        dashParticleSystem.Play();
    }

    public void StopDashEffect()
    {
        dashParticleSystem.Stop();
    }
}
