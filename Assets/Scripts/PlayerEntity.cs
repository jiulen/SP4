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
    private GameObject equipped;
    public GameObject activeWeapon;
    private GameObject previousWeapon;
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
    public GameObject PlayerCanvasParentPF;
    //public GameObject UIWeaponWheelPF;
    //public GameObject PlayerStatsCanvasPF;
    public GameObject StaminaCanvasPF;
    //public GameObject CrosshairCanvasPF;
    //public GameObject DeathCanvasPF;
    //public GameObject CameraEffectsCanvasPF;
    public Image DamageIndicatorImagePF;
    public GameObject KillFeedPrefab;

    void Awake()
    {
       
        FPSScript = this.GetComponent<FPS>();
        base.Start();
        equipped = transform.Find("Equipped").gameObject;
        GameObject rightHand = equipped.transform.Find("Right Hand").gameObject;
        for (int i = 0; i != rightHand.transform.childCount; i++)
        {
            equippedWeaponList[i] = rightHand.transform.GetChild(i).gameObject;
            if (i != 0)
                equippedWeaponList[i].SetActive(false);
        }
        activeWeapon = equippedWeaponList[0];
        previousWeapon = activeWeapon;
        uiKillFeedCanvas = GameObject.Find("Canvas/KillerFeedUI");
    }

    void Start()
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
    public void SpawnKillFeed(GameObject playerkills, GameObject playerdeath, Sprite typeofWeapon)
    {
        GameObject spawnFeed = Instantiate(KillFeedPrefab, uiKillFeedCanvas.transform);
        spawnFeed.transform.Find("PlayerKill").GetComponent<Text>().text = playerkills.name;
        spawnFeed.transform.Find("PlayerDeath").GetComponent<Text>().text = playerdeath.name;
        spawnFeed.transform.Find("KillerWeapon").GetComponent<Image>().sprite = typeofWeapon;
        spawnFeed.transform.parent = uiKillFeedCanvas.transform;

       //if (killfeedlist)
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        //Debug.Log(Health);
        if (!FPSScript.IsOwner && !FPSScript.debugBelongsToPlayer) return;

        // For debugging
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.LogError("RESETTING UI");
            Destroy(uiStaminaCanvas);
            uiStaminaCanvas = Instantiate(StaminaCanvasPF, this.transform);
            uiStaminaCanvas.GetComponent<StaminaUI>().InitBars();
        }

        if (Health <= 0)
        {
            UpdateDead();
            uiDeathCanvas.GetComponentInChildren<Text>().text = ((int)currentrespawnelaspe).ToString();
            uiDeathCanvas.SetActive(true);
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
    public override void TakeDamage(float hp, Vector3 dir, GameObject source, GameObject weaponUsed)
    {
        //Debug.Log(cameraEffectsCanvas);
        //Image dmgImg = Instantiate(DamageIndicatorImagePF) as Image;
        //dmgImg.GetComponent<DamageIndicator>().SetSourcePos(dir);
        //dmgImg.transform.parent = cameraEffectsCanvas.transform;
        //cameraEffectBlood.GetComponent<BloodEffects>().ResetStartDuration();
        //Debug.Log(cameraEffectBlood);
        SetLastTouch(source);
        SetHealth(GetHealth() - hp);

        //if (Health <= 0)

        Debug.Log(weaponUsed);

        Debug.Log(source);
        SpawnKillFeed(source, this.gameObject, weaponUsed.GetComponent<WeaponBase>().WeaponIcon);
    }


    public void UpdateDead()
    {        
        if ((int)currentrespawnelaspe <= 0)
        {
            FPSScript.enabled = true;
            transform.GetComponent<Teleport>().enabled = true;
            transform.GetComponent<PickupCollectibles>().enabled = true;
            transform.Find("Head").gameObject.SetActive(true);
            SetHealth(MaxHealth);
            currentrespawnelaspe = respawncountdown;
        }
        else
        {
            FPSScript.enabled = false;
            transform.Find("Head").gameObject.SetActive(false);
            transform.GetComponent<Teleport>().enabled = false;
            transform.GetComponent<PickupCollectibles>().enabled = false;
            currentrespawnelaspe -= Time.deltaTime;
        }
        Debug.Log("You are killed by " + GetLastTouch());
    }

    public string GetWeaponName(int idx)
    {
        return equippedWeaponList[idx].name;
    }

    public GameObject GetCrosshairCanvas()
    {
        Debug.LogWarning(crosshairCanvas);
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
}
