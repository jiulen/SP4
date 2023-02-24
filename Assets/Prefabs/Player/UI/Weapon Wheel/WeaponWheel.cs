using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheel : MonoBehaviour
{
    private FPS FPSScript;
    private PlayerEntity PlayerEntityScript;

    // Used to disable UI without disabling this script
    GameObject uiParent;
    GameObject selectorPivot;
    GameObject wheelSegmentsParent;
    public GameObject[] equippedWeaponList = new GameObject[3];

    // Weapon wheel
    Vector3 saveMousePostion;
    public int segmentNum = 3;
    public float radius;
    public float startingAngle = 0; //Right, anti-clockwise

    private int selectedWeapon = 0;
    public float weaponWheelDelay = 0.2f;
    private double weaponWheelElapsed = 0;

    void Start()
    {

        uiParent = this.transform.GetChild(0).gameObject;
        selectorPivot = uiParent.transform.Find("Selector Pivot").gameObject;
        wheelSegmentsParent = uiParent.transform.Find("Wheel Segments Parent").gameObject;

        FPSScript = this.transform.parent.parent.GetComponent<FPS>(); // this > canvas parent > player
        PlayerEntityScript = this.transform.parent.parent.GetComponent<PlayerEntity>(); // this > canvas parent > player

        this.GetComponent<Canvas>().worldCamera = FPSScript.camera;
        this.GetComponent<Canvas>().planeDistance = 0.11f;
        equippedWeaponList = PlayerEntityScript.equippedWeaponList;

        for (int i = 0; i != segmentNum; i++)
        {
            if (equippedWeaponList[i] != null)
                wheelSegmentsParent.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = equippedWeaponList[i].GetComponent<WeaponBase>().WeaponIcon;
        }

    }

    void Update()
    {

        if (Input.GetKey(KeyCode.Q))
        {
            weaponWheelElapsed += Time.deltaTime;

            if (weaponWheelElapsed >= weaponWheelDelay)
            {
                FPSScript.cameraMoveEnabled = false;
                uiParent.SetActive(true);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.None;
                Vector2 currentMousePosRelativeToStart = Input.mousePosition - saveMousePostion;
                float mouseAngle = Mathf.Atan2(currentMousePosRelativeToStart.x, currentMousePosRelativeToStart.y) * Mathf.Rad2Deg; // Convert the vector to an angle in degrees
                float normalizedMouseAngle = mouseAngle;
                if (normalizedMouseAngle < 0)
                {
                    normalizedMouseAngle += 360;
                }
                float mouseaA = 360 - mouseAngle;
                selectorPivot.transform.localRotation = Quaternion.Euler(0, 0, -mouseAngle + 90);
                for (int i = 0; i != segmentNum; i++)
                {
                    float segmentArc = (360 / segmentNum);
                    float angle = 0 + (segmentArc * i) + startingAngle;
                    wheelSegmentsParent.transform.GetChild(i).transform.localRotation = Quaternion.Euler(0, 0, 360 - angle);

                    float lowerAngle = angle - segmentArc / 2;
                    if (lowerAngle < 0)
                        lowerAngle += 360;

                    float upperAngle = lowerAngle + segmentArc;

                    if (IsAngleBetween(normalizedMouseAngle, lowerAngle, upperAngle))
                    {
                        wheelSegmentsParent.transform.GetChild(i).transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                        selectedWeapon = i;
                    }
                    else
                    {
                        wheelSegmentsParent.transform.GetChild(i).transform.localScale = new Vector3(1f, 1f, 1f);

                    }
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            if (weaponWheelElapsed >= weaponWheelDelay)
            {
                PlayerEntityScript.SetActiveWeapon(selectedWeapon);
            }
            else
                PlayerEntityScript.SetActiveWeapon(-1);
            weaponWheelElapsed = 0;
        }
        else
        {
            saveMousePostion = Input.mousePosition;
            FPSScript.cameraMoveEnabled = true;
            uiParent.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

     


    }

    public bool IsAngleBetween(float angleToCheck, float angleStart, float angleEnd)
    {
        float angleDiff = Mathf.Abs(angleEnd - angleStart);
        float angleToCheckDiff = Mathf.Abs((angleToCheck - angleStart + 360) % 360);
        if (angleToCheckDiff > angleDiff) 
            return false;
        return true;
        
    }
}
