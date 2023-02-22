using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PlayerEntity : EntityBase
{
    public GameObject[] Weapons;
    private float respawncountdown = 5, currentrespawnelaspe;
    private int currentIdx;
    public Image CurrentWeaponIcon;
    public Image DamageIndicator;
    public GameObject CameraEffectsUIManager;
    public Image Injured, BloodEffect;
    public GameObject WheelManagerUI;

    private GameObject DeathUI;
    // Start is called before the first frame update
    void Start()
    {
        currentIdx = 0;
        base.Start();

        for (int i = 0; i < WheelManagerUI.GetComponentsInChildren<Button>().Length; i++)
        {
            var index = i;
            Button items = WheelManagerUI.GetComponentsInChildren<Button>()[i];
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
        DeathUI = transform.Find("Canvas/DeathUI").gameObject;
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


        if (Input.GetKey(KeyCode.E))
        {
            WheelManagerUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            WheelManagerUI.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }



        foreach (GameObject weapon in Weapons)
        {
            if (Weapons[currentIdx] == weapon && Health > 0)
            {
                weapon.SetActive(true);
            }
            else
            {
                weapon.SetActive(false);
            }
        }

        if (Health <= 0)
        {
            UpdateDead();
            DeathUI.SetActive(true);
        }
        else
        {
            DeathUI.SetActive(false);
        }

        //weapon icon and inside the wheel
        if (Weapons[currentIdx].GetComponent<WeaponBase>() != null)
            CurrentWeaponIcon.sprite = Weapons[currentIdx].GetComponent<WeaponBase>().WeaponIcon;

        for (int i = 0; i < WheelManagerUI.GetComponentsInChildren<Button>().Length; i++)
        {
            Button items = WheelManagerUI.GetComponentsInChildren<Button>()[i];
            items.gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Weapons[i].GetComponent<WeaponBase>().WeaponIcon;
        }


        Color currAlpha = Injured.color;
        currAlpha.a = 1 - (Health / MaxHealth);
        Injured.color = currAlpha;

    }


    public override void TakeDamage(float hp, Vector3 dir)
    {
        Image dmgImg = Instantiate(DamageIndicator) as Image;
        dmgImg.GetComponentInChildren<DamageIndicator>().SetSourcePos(dir);
        dmgImg.transform.SetParent(CameraEffectsUIManager.transform, false);

        BloodEffect.GetComponent<BloodEffects>().ResetStartDuration();

        SetHealth(GetHealth() - hp);
    }

    public void UpdateDead()
    {
        DeathUI.GetComponentInChildren<Text>().text = ((int)currentrespawnelaspe).ToString();
        
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
}
