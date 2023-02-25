using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoWheel : MonoBehaviour
{
    private FPS FPSScript;
    private PlayerEntity PlayerEntityScript;
    private DeRolo DeRoloScript;

    // Used to disable UI without disabling this script
    GameObject uiParent;

    GameObject selectorPivot;
    GameObject wheelSegmentsParent;
    GameObject originalSegmentPivot;
    GameObject centre;

    // Ammo wheel
    Vector3 saveMousePostion;
    private int segmentNum = 3;
    public float radius;
    public float startingAngle = 0; //Right, anti-clockwise
    public float segmentGapAngle = 5;
    public float centreRadius = 25;
    public float centreToSegmentGap = 5;
    public float totalRadius = 100;

    public float ammoWheelDelay = 0.2f;
    private double ammoWheelElapsed = 0;

    public bool wheelActive = false;

    void Start()
    {

        uiParent = this.transform.GetChild(0).gameObject;
        selectorPivot = uiParent.transform.Find("Selector Pivot").gameObject;
        wheelSegmentsParent = uiParent.transform.Find("Wheel Segments Parent").gameObject;
        originalSegmentPivot = wheelSegmentsParent.transform.GetChild(0).gameObject;
        centre = uiParent.transform.Find("Centre").gameObject;

        DeRoloScript = this.transform.parent.GetComponent<DeRolo>();

        FPSScript = DeRoloScript.owner.GetComponent<FPS>(); // this > canvas parent > player
        PlayerEntityScript = FPSScript.GetComponent<PlayerEntity>(); // this > canvas parent > player
   
        this.GetComponent<Canvas>().worldCamera = FPSScript.camera;
        this.GetComponent<Canvas>().planeDistance = 0.2f;

        segmentNum = Enum.GetNames(typeof(DeRolo.BulletTypes)).Length - 1;
        InitMeshes();   
    }

    public void InitMeshes()
    {
        // Change selector pivot distance based on new radius
        selectorPivot.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(centreRadius + centreToSegmentGap/2, 0, 0);

        // Initialise centre
        centre.GetComponent<ProudLlama.CircleGenerator.FillCircleGenerator>().CircleData = new ProudLlama.CircleGenerator.CircleData(centreRadius, 360, 0, 32, true);

        //foreach(Transform child in wheelSegmentsParent.transform)
        //{
        //    if (child != originalSegmentPivot)
        //        Destroy(child);
        //}



        // Skip bullet type NONE
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
            ProudLlama.CircleGenerator.StrokeCircleGenerator iterateSegmentCircle = iterateSegment.transform.GetChild(0).GetComponent<ProudLlama.CircleGenerator.StrokeCircleGenerator>();
            float arcAngle = 360 / segmentNum - segmentGapAngle;
            iterateSegmentCircle.CircleData = new ProudLlama.CircleGenerator.CircleData(centreRadius + centreToSegmentGap, arcAngle, 360 - arcAngle/2, 32, true);
            iterateSegmentCircle.StrokeData = new ProudLlama.CircleGenerator.StrokeData(strokeSize, false);
            iterateSegmentCircle.Generate();

            // Recalculate normals to get the material to work
            iterateSegmentCircle.GetComponent<MeshFilter>().mesh.RecalculateNormals();

            iterateSegment.transform.rotation = Quaternion.Euler(0, 0, startingAngle + 360 / segmentNum * (i));
            GameObject ammoImage = iterateSegment.transform.GetChild(0).GetChild(0).gameObject;
            ammoImage.GetComponent<RectTransform>().localPosition = new Vector3(0, centreRadius + centreToSegmentGap + strokeSize/ 2, -0.1f);
            ammoImage.GetComponent<Image>().color = DeRoloScript.bulletColors[i + 1];
        }
    }

    void Update()
    {

        if (Input.GetKey(KeyCode.R))
        {
            ammoWheelElapsed += Time.deltaTime;

            if (ammoWheelElapsed >= ammoWheelDelay)
            {
                wheelActive = true;
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
                        if (Input.GetButtonDown("Fire1"))
                        {
                            DeRoloScript.reloadQueue.Add((DeRolo.BulletTypes)(i + 1));
                        }
                    }
                    else
                    {
                        wheelSegmentsParent.transform.GetChild(i).transform.localScale = new Vector3(1f, 1f, 1f);

                    }
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            if (ammoWheelElapsed >= ammoWheelDelay)
            {
                DeRoloScript.Reload();
            }
            else
            {

            }
             ammoWheelElapsed = 0;
             wheelActive = false;
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
