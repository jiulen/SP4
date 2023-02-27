using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class WeaponWheelV2 : MonoBehaviour
{
    private FPS FPSScript;
    private PlayerEntity PlayerEntityScript;

    // Used to disable UI without disabling this script
    GameObject uiParent;
    GameObject selectorPivot;
    GameObject wheelSegmentsParent;
    GameObject originalSegmentPivot;
    GameObject centre;

    public GameObject[] equippedWeaponList = new GameObject[3];

    // Weapon wheel
    Vector3 saveMousePostion;
    public int segmentNum = 3;
    public float radius;
    public float startingAngle = 0; //Right, anti-clockwise
    public float segmentGapAngle = 5;
    public float centreRadius = 50;
    public float centreToSegmentGap = 5;
    public float totalRadius = 100;

    private int selectedWeapon = 0;
    public float weaponWheelDelay = 0.2f;
    private double weaponWheelElapsed = 0;

    void Start()
    {

        uiParent = this.transform.GetChild(0).gameObject;
        selectorPivot = uiParent.transform.Find("Selector Pivot").gameObject;
        wheelSegmentsParent = uiParent.transform.Find("Wheel Segments Parent").gameObject;
        originalSegmentPivot = wheelSegmentsParent.transform.GetChild(0).gameObject;
        centre = uiParent.transform.Find("Centre").gameObject;

        FPSScript = this.transform.parent.parent.GetComponent<FPS>(); // this > canvas parent > player
        PlayerEntityScript = this.transform.parent.parent.GetComponent<PlayerEntity>(); // this > canvas parent > player

        this.GetComponent<Canvas>().worldCamera = FPSScript.camera;
        this.GetComponent<Canvas>().planeDistance = 0.2f;
        equippedWeaponList = PlayerEntityScript.equippedWeaponList;

        //InitMeshes();

        //for (int i = 0; i != segmentNum; i++)
        //{
        //    if (equippedWeaponList[i] != null)
        //        wheelSegmentsParent.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = equippedWeaponList[i].GetComponent<WeaponBase>().WeaponIcon;
        //}

    }

    public void InitMeshes()
    {
        // Change selector pivot distance based on new radius
        selectorPivot.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(centreRadius + centreToSegmentGap/2, 0, 0);

        // Initialise centre
        centre.GetComponent<ProudLlama.CircleGenerator.FillCircleGenerator>().CircleData = new ProudLlama.CircleGenerator.CircleData(centreRadius, 360, 0, 32, true);
        centre.GetComponent<ProudLlama.CircleGenerator.FillCircleGenerator>().Generate();

        //foreach(Transform child in wheelSegmentsParent.transform)
        //{
        //    if (child != originalSegmentPivot)
        //        Destroy(child);
        //}

        for (int i = 0; i != segmentNum; i++)
        {
            GameObject iterateSegment = originalSegmentPivot;
            // Skip instantiation of the first segment as it already exists
            if (i != 0)
            {
                iterateSegment = Instantiate(originalSegmentPivot, wheelSegmentsParent.transform);
            }

            // Set the angle to 300 so that it is oriented to the top. We rotate the pivot instead to get it each segment where we want
            float strokeSize = totalRadius - centreRadius - centreToSegmentGap;
            Debug.Log(strokeSize);
            ProudLlama.CircleGenerator.StrokeCircleGenerator iterateSegmentCircle = iterateSegment.transform.GetChild(0).GetComponent<ProudLlama.CircleGenerator.StrokeCircleGenerator>();
            float arcAngle = 360 / segmentNum - segmentGapAngle;
            iterateSegmentCircle.CircleData = new ProudLlama.CircleGenerator.CircleData(centreRadius + centreToSegmentGap, arcAngle, 360 - arcAngle / 2, 32, true);
            iterateSegmentCircle.StrokeData = new ProudLlama.CircleGenerator.StrokeData(strokeSize, false);
            iterateSegmentCircle.Generate();

            // Recalculate normals to get the material to work
            iterateSegmentCircle.GetComponent<MeshFilter>().mesh.RecalculateNormals();

            iterateSegment.transform.rotation = Quaternion.Euler(0, 0, startingAngle + 360 / segmentNum * i);
            GameObject weaponImage = iterateSegment.transform.GetChild(0).GetChild(0).gameObject;
            weaponImage.GetComponent<RectTransform>().localPosition = new Vector3(0, centreRadius + centreToSegmentGap + strokeSize/ 2, -0.1f);
            if(i != equippedWeaponList.Length)
                if (equippedWeaponList[i] != null)
                    weaponImage.GetComponent<Image>().sprite = equippedWeaponList[i].GetComponent<WeaponBase>().WeaponIcon;
        }
    }

    void Update()
    {
        if (transform.parent.parent.GetComponent<NetworkObject>().IsOwner)
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
                //Cursor.visible = false;
                //Cursor.lockState = CursorLockMode.Locked;
            }
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
