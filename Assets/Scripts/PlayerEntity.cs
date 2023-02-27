using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

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
    private int kills = 0, deaths = 0;

    void Awake()
    {
        FPSScript = this.GetComponent<FPS>();
        base.Start();
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
        dashEffectCanvas.GetComponent<Canvas>().planeDistance = 0.1f;
    }

    void Start()
    {
        InitialiseUI();
        currentrespawnelaspe = respawncountdown;
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
            uiKillFeedCanvas = GameObject.Find("Canvas/KillerFeedUI");
            dashEffectCanvas.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            dashEffectCanvas.GetComponent<Canvas>().planeDistance = 50f;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
    private void UpdateKillFeedClientRpc(ulong networkObject, ulong networkObject2, ulong playerkill, ulong playerdeath)
    {
        GameObject me = null, enemy = null;
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerkill, out NetworkObject networkObjectplayerkill))
            me = networkObjectplayerkill.gameObject;
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerdeath, out NetworkObject networkObjectenemy))
            enemy = networkObjectenemy.gameObject;

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObject, out NetworkObject networkObject1))
        {
            networkObject1.gameObject.transform.Find("PlayerKill").GetComponent<Text>().text = me.name;
            networkObject1.gameObject.transform.Find("PlayerDeath").GetComponent<Text>().text = enemy.name;
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObject2, out NetworkObject networkObject22))
            {
                networkObject1.gameObject.transform.Find("KillerWeapon").GetComponent<Image>().sprite = networkObject22.gameObject.GetComponent<WeaponBase>().WeaponIcon;
            }

        }

        me.GetComponent<PlayerEntity>().kills++;
        enemy.GetComponent<PlayerEntity>().deaths++;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnKillFeedServerRpc(ulong playerkills, ulong playerdeath, ulong typeofWeapon)
    {
        GameObject spawnFeed = Instantiate(KillFeedPrefab, uiKillFeedCanvas.transform);
        spawnFeed.GetComponent<NetworkObject>().Spawn();
        UpdateKillFeedClientRpc(spawnFeed.GetComponent<NetworkObject>().NetworkObjectId, typeofWeapon, playerkills, playerdeath);
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
            killfeed.transform.SetParent(uiKillFeedCanvas.transform);
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
        uiPlayerStatsCanvas.transform.Find("MyScore/Text").GetComponent<Text>().text = kills.ToString();
        uiPlayerStatsCanvas.transform.Find("EScore/Text").GetComponent<Text>().text = deaths.ToString();
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
    private void TakeDmgServerRpc(float hp, Vector3 dir, ulong source)
    {
        if (equippedWeaponList.Length > 0)
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
        }
    }

    public override void TakeDamage(float hp, Vector3 dir, GameObject source, GameObject weaponUsed)
    {
        TakeDmgServerRpc(hp, dir, source.gameObject.GetComponent<NetworkObject>().NetworkObjectId);

        if (Health <= 0)
        {
            SpawnKillFeedServerRpc(source.gameObject.GetComponent<NetworkObject>().NetworkObjectId, this.gameObject.GetComponent<NetworkObject>().NetworkObjectId, weaponUsed.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
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
        //activeWeapon.SetActive(false);
        SetWeaponActiveServerRpc(activeWeapon.GetComponent<NetworkObject>().NetworkObjectId, false);

        // Swap to previous weapon
        if (i == -1)
        {
            GameObject tempReference = previousWeapon;
            //previousWeapon.SetActive(true);
            SetWeaponActiveServerRpc(previousWeapon.GetComponent<NetworkObject>().NetworkObjectId, true);

            previousWeapon = activeWeapon;
            activeWeapon = tempReference;
        }
        else
        {
            //equippedWeaponList[i].SetActive(true);
            SetWeaponActiveServerRpc(equippedWeaponList[i].GetComponent<NetworkObject>().NetworkObjectId, true);
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

    [ServerRpc]
    private void SetWeaponActiveServerRpc(ulong weaponID, bool active)
    {
        SetWeaponActiveClientRpc(weaponID, active);
    }

    [ClientRpc]
    private void SetWeaponActiveClientRpc(ulong weaponID, bool active)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(weaponID, out NetworkObject weaponObject))
            weaponObject.gameObject.SetActive(active);
    }
}
