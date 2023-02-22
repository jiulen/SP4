using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeRolo : WeaponBase
{
    GameObject ui;
    Canvas uiCanvas;

    GameObject uiChamberParent;
    public List<GameObject> uiChamberList = new List<GameObject>();
    public List<Image> uiImageChamberList = new List<Image>();


    public enum BulletTypes
    {
        NONE = 0,
        NORMAL = 1,
        SHORTEXPLOSIVE = 2,
        MEDIUMEXPLOSIVE = 3,
        GRAPPLE = 4
    };

    public List<Color> bulletColors = new List<Color>()
    {
        Color.black,
        Color.yellow,
        new Color(255, 60, 0),
        Color.red,
        Color.blue
    };

    public List<BulletTypes> cylinder = new List<BulletTypes>();
    private int cylinderSize = 6;
    void Start()
    {
        for(int i = 0; i != cylinderSize; i++)
        {
            cylinder.Add(BulletTypes.NONE);
        }

        ui = transform.Find("UI Canvas").gameObject;
        uiCanvas = ui.GetComponent<Canvas>();

        uiChamberParent = ui.transform.Find("Cylinder/Chamber Parent").gameObject;

        for(int i = 0; i != cylinderSize; i++)
        {
            uiChamberList.Add(uiChamberParent.transform.GetChild(i).gameObject);
            uiImageChamberList.Add(uiChamberList[i].GetComponent<Image>());
        }

    }

    void Update()
    {
        for (int i = 0; i != cylinderSize; i++)
        {
            Debug.Log(i);

            //Debug.Log((int)cylinder[i]);
            Color color = bulletColors[(int)cylinder[i]];
            uiImageChamberList[i].color = color;
        }
    }
}
