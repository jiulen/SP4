using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private Slider slider;
    [SerializeField] Text hbartext;
    [SerializeField] Image hBarFill;
    private EntityBase player;
   // private Image Injured;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        player = transform.parent.parent.GetComponent<PlayerEntity>();
        slider.maxValue = slider.value = player.MaxHealth;
        //Injured = transform.parent.Find("Injured").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        float percentage = (player.GetHealth() / player.MaxHealth);
        //player.TakeDamage(Time.deltaTime); // testing
        UpdateSlider();

        if (percentage <= 0.75f)
            hBarFill.color = new Color32(85, 229, 106, 255);
        if (percentage <= 0.5f)
            hBarFill.color = new Color32(255, 255, 0, 255);
        if (percentage <= 0.25f)
            hBarFill.color = new Color32(255, 0, 0, 255);


        hbartext.text = ((int)(percentage * 100f)).ToString() + "%";
    }

    void UpdateSlider()
    {
        if (!slider)
            return;

        slider.value = player.GetHealth();
    }
}
