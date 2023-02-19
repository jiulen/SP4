using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
public class PlayerEntity : EntityBase
{
    public GameObject[] Weapons;
    private int currentIdx;
    public Image CurrentWeaponIcon;
    public Image DamageIndicator;
    public GameObject CameraEffectsUIManager;
    public Image Injured, BloodEffect;

    // Start is called before the first frame update
    void Start()
    {
        currentIdx = 0;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.M))
        {
            currentIdx++;
            if (currentIdx > Weapons.Length - 1)
                currentIdx = 0;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            currentIdx--;
            if (currentIdx < 0)
                currentIdx = Weapons.Length - 1;
        }

        foreach(GameObject weapon in Weapons)
        {
            if (Weapons[currentIdx] == weapon)
            {
                weapon.SetActive(true);
            }
            else
            {
                weapon.SetActive(false);
            }
        }

        if (Weapons[currentIdx].GetComponent<WeaponBase>() != null)
            CurrentWeaponIcon.sprite = Weapons[currentIdx].GetComponent<WeaponBase>().WeaponIcon;

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

        if (Health <= 0)
        {
            //Destroy(transform.root.gameObject);
        }
    }
}
