using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    // For debugging. To be set in the editor during runtime. Will call the Init function once, and then will be set back to false
    public bool reInit = false;


    public int staminaMax = 3;
    public float spaceBetweenBars = 5;
    public float totalWidth = 50;
    public float height = 10;
    public float yAxisOffset = -10;
    private GameObject owner;
    private float staminaAmount;
    private GameObject barReference;
    void Awake()
    {
        owner = this.transform.parent.gameObject;
        staminaMax = owner.GetComponent<FPS>().staminaMax;
        barReference = this.transform.GetChild(0).gameObject;

    }

    public void InitBars()
    {
        // For debugging purposes, we clear all the cloned children and then reinstantiate them
        //for(int i = 1; i != this.transform.childCount; i++)
        //{
        //    Destroy(this.transform.GetChild(i));
        //}

        float barWidth = (totalWidth - (spaceBetweenBars * (staminaMax - 1))) / staminaMax;
        float startingXPos = -(totalWidth / 2) + barWidth / 2; 
        for (int i = 0; i != staminaMax; i++)
        {
            if (i != 0)
            {
                float xPos = startingXPos + (spaceBetweenBars + barWidth) * i;
                GameObject newBar = Instantiate(barReference, this.transform);
                RectTransform newBarRect = newBar.GetComponent<RectTransform>();
                //newBarRect.rect.Set(xPos, 0, barWidth, height);
                newBarRect.sizeDelta = new Vector2(barWidth, height);
                newBarRect.anchoredPosition = new Vector3(xPos, yAxisOffset, 0);
            }
            else
            {
                Debug.Log(barReference);
                //barReference.GetComponent<RectTransform>().rect.Set(startingXPos, 0, barWidth, height);
                barReference.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, height);
                barReference.GetComponent<RectTransform>().anchoredPosition = new Vector3(startingXPos, yAxisOffset, 0);
            }

        }
    }

    void Update()
    {
        // For Debugging
        if(reInit)
        {
            InitBars();
        }
        reInit = false;


    }

    public void UpdateStamina(float currentAmount)
    {
        staminaAmount = currentAmount;
        int i = 0;
        foreach (Transform child in this.transform)
        {
            Debug.Log(i);
            Slider slider = child.GetComponent<Slider>();
            if (slider != null)
            {
                float segmentedValue = currentAmount - i;
                slider.value = segmentedValue;
                if (segmentedValue > 0.5)
                    slider.transform.Find("Fill Area").GetChild(0).GetComponent<Image>().color = Color.green;
                else
                    slider.transform.Find("Fill Area").GetChild(0).GetComponent<Image>().color = new Color(255,191,0,255);

                i++;
            }

        }
    }
}
