using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

// Player script for UI, playerstats 
public class PlayerEntity : EntityBase
{
    private FPS FPSScript;
    public GameObject[] equippedWeaponList = new GameObject[3];
    private GameObject equipped;
    private float respawncountdown = 5, currentrespawnelaspe;
    private int currentIdx;
    private Image uiCurrentWeaponIcon;
    private Image cameraEffectInjured, cameraEffectBlood;
    private GameObject uiWeaponWheelCanvas;
    private GameObject uiPlayerStatsCanvas;
    private GameObject uiDeathCanvas;
    private GameObject uiStaminaCanvas;
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
    //public GameObject MiniMapPF;
    public Image DamageIndicatorImagePF;

    void Awake()
    {
        FPSScript = this.GetComponent<FPS>();
        currentIdx = 0;
        base.Start();
        equipped = transform.Find("Equipped").gameObject;
        GameObject rightHand = equipped.transform.Find("Right Hand").gameObject;

        if (!FPSScript.IsOwner && !FPSScript.debugBelongsToPlayer) return;



        for (int i = 0; i != rightHand.transform.childCount; i++)
        {
            equippedWeaponList[i] = rightHand.transform.GetChild(i).gameObject;
        }

        playerCanvasParent = Instantiate(PlayerCanvasParentPF, this.transform);
        uiWeaponWheelCanvas = playerCanvasParent.transform.Find("Weapon Wheel Canvas").gameObject;
        uiWeaponWheelCanvas.GetComponent<Canvas>().worldCamera = FPSScript.camera;

        uiDeathCanvas = playerCanvasParent.transform.Find("Death Canvas").gameObject;
        uiPlayerStatsCanvas = playerCanvasParent.transform.Find("Player Stats Canvas").gameObject;
        //Instantiate(MiniMapPF, this.transform);

        uiCurrentWeaponIcon = uiPlayerStatsCanvas.transform.Find("CurrentWeaponImage").GetComponent<Image>();
        cameraEffectsCanvas = playerCanvasParent.transform.Find("Camera Effects Canvas").gameObject;
        Debug.Log(cameraEffectsCanvas);
        cameraEffectInjured = cameraEffectsCanvas.transform.Find("Injured").GetComponent<Image>();
        cameraEffectBlood = cameraEffectsCanvas.transform.Find("Blood").GetComponent<Image>();

        uiStaminaCanvas = playerCanvasParent.transform.Find("Stamina Canvas").gameObject;
        uiStaminaCanvas.GetComponent<StaminaUI>().InitBars();

        crosshairCanvas = playerCanvasParent.transform.Find("Custom Crosshair Canvas").gameObject;
        crosshairCanvas.GetComponent<CustomCrosshair>().doUpdate = true;

        for (int i = 0; i < uiWeaponWheelCanvas.GetComponentsInChildren<Button>().Length; i++)
        {
            var index = i;
            Button items = uiWeaponWheelCanvas.GetComponentsInChildren<Button>()[i];
            items.onClick.AddListener(() => UpdateWheelManager(index));

            EventTrigger trigger = items.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((eventData) => { OnPointerEnterDelegate(eventData, items, index); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerExit;
            entry.callback.AddListener((eventData) => { OnPointerExitDelegate(eventData, items, index); });
            trigger.triggers.Add(entry);
        }
        //DeathUI = transform.Find("Canvas/DeathUI").gameObject;
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

        if (Input.GetKey(KeyCode.Q))
        {
            uiWeaponWheelCanvas.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            uiWeaponWheelCanvas.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }



        //foreach (GameObject weapon in equippedWeaponList)
        //{
        //    if (equippedWeaponList[currentIdx] == weapon && Health > 0)
        //    {
        //        weapon.SetActive(true);
        //    }
        //    else
        //    {
        //        weapon.SetActive(false);
        //    }
        //}

        if (Health <= 0)
        {
            UpdateDead();
            uiDeathCanvas.SetActive(true);
        }
        else
        {
            uiDeathCanvas.SetActive(false);
        }

        //weapon icon and inside the wheel
        if (equippedWeaponList[currentIdx] != null)
           uiCurrentWeaponIcon.sprite = equippedWeaponList[currentIdx].GetComponent<WeaponBase>().WeaponIcon;

        for (int i = 0; i < uiWeaponWheelCanvas.GetComponentsInChildren<Button>().Length; i++)
        {
            Button items = uiWeaponWheelCanvas.GetComponentsInChildren<Button>()[i];
            if(equippedWeaponList[i] != null)
                items.gameObject.transform.GetChild(0).GetComponent<Image>().sprite = equippedWeaponList[i].GetComponent<WeaponBase>().WeaponIcon;
        }


        Color currAlpha = cameraEffectInjured.color;
        currAlpha.a = 1 - (Health / MaxHealth);
        cameraEffectInjured.color = currAlpha;

        uiStaminaCanvas.GetComponent<StaminaUI>().UpdateStamina(FPSScript.staminaAmount);

    }


    public override void TakeDamage(float hp, Vector3 dir)
    {
        //Debug.Log(cameraEffectsCanvas);
        //Image dmgImg = Instantiate(DamageIndicatorImagePF) as Image;
        //dmgImg.GetComponent<DamageIndicator>().SetSourcePos(dir);
        //dmgImg.transform.parent = cameraEffectsCanvas.transform;
        //cameraEffectBlood.GetComponent<BloodEffects>().ResetStartDuration();
        //Debug.Log(cameraEffectBlood);
        SetHealth(GetHealth() - hp);
    }

    public void UpdateDead()
    {
        uiDeathCanvas.GetComponentInChildren<Text>().text = ((int)currentrespawnelaspe).ToString();
        
        if ((int)currentrespawnelaspe <= 0)
        {
            transform.GetComponent<FPS>().enabled = true;
            transform.GetComponent<Teleport>().enabled = true;
            transform.GetComponent<PickupCollectibles>().enabled = true;
            transform.Find("Head").gameObject.SetActive(true);
            SetHealth(MaxHealth);
            currentrespawnelaspe = respawncountdown;
        }
        else
        {
            transform.GetComponent<FPS>().enabled = false;
            transform.Find("Head").gameObject.SetActive(false);
            transform.GetComponent<Teleport>().enabled = false;
            transform.GetComponent<PickupCollectibles>().enabled = false;
            currentrespawnelaspe -= Time.deltaTime;
        }
    }

    private void UpdateWheelManager(int idx)
    {
        currentIdx = idx;
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
}
