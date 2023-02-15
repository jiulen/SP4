using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private float Health;
    public float MaxHealth;
    private Slider slider;
    [SerializeField] Text hbartext;
    [SerializeField] Image hBarFill;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        SetMaxHealth(MaxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        float percentage = (Health / MaxHealth);
        Health -= Time.deltaTime;
        SetHealth(Health);

        if (Health > MaxHealth)
            Health = MaxHealth;
        if (Health < 0)
            Health = 0;



        if (percentage <= 0.75f)
            hBarFill.color = new Color32(85, 229, 106, 255);
        if (percentage <= 0.5f)
            hBarFill.color = new Color32(255, 255, 0, 255);
        if (percentage <= 0.25f)
            hBarFill.color = new Color32(255, 0, 0, 255);


        hbartext.text = ((int)(percentage * 100f)).ToString() + "%";
    }

    public void SetMaxHealth(float hp)
    {
        if (!slider)
            slider = GetComponent<Slider>();
        slider.maxValue = hp;
        slider.value = hp;
        Health = MaxHealth = hp;
    }

    public void SetHealth(float hp)
    {
        Health = hp;
        slider.value = hp;
    }

    public float GetHealth()
    {
        return Health;
    }
}
