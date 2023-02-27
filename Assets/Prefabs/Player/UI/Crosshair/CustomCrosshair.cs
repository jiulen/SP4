using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CustomCrosshair : MonoBehaviour
{
    public float centreGap = 5;
    public float xLineLength = 10;
    public float xLineThickness = 2;
    public float yLineLength = 4;
    public float yLineThickness = 2;
    public float centreThickness = 2;
    public float circleThickness = 3;
    public bool enableCircle = false;
    public bool enableLines = true;
    public bool enableCentre = true;
    public bool dynamic = true;

    public Color color = Color.green;

    // To be set to true whenever the crosshair values are changed.
    public bool doUpdate = true;

    private RectTransform lineXP;
    private RectTransform lineXN;
    private RectTransform lineYP;
    private RectTransform lineYN;
    private RectTransform lineCentre;

    private Transform circle;
    private ProudLlama.CircleGenerator.StrokeCircleGenerator circleScript;
    public GameObject CrosshairGo;
    private Canvas canvas;

    private bool menuActive = false;
    public GameObject menuParent;

    public Slider sliderCentreGap;
    public Slider sliderHorizontalLength;
    public Slider sliderHorizontalThickness;
    public Slider sliderVerticalLength;
    public Slider sliderVeritcalThickness;

    public Slider sliderR;
    public Slider sliderG;
    public Slider sliderB;

    public Toggle toggleCircle;
    public Toggle toggleLines;
    public Toggle toggleCentreDot;
    public Toggle toggleDynamic;

    void Start()
    {
        canvas = CrosshairGo.GetComponent<Canvas>();

        lineXP = CrosshairGo.transform.Find("LineXP").GetComponent<RectTransform>();
        lineXN = CrosshairGo.transform.Find("LineXN").GetComponent<RectTransform>();
        lineYP = CrosshairGo.transform.Find("LineYP").GetComponent<RectTransform>();
        lineYN = CrosshairGo.transform.Find("LineYN").GetComponent<RectTransform>();
        lineCentre = CrosshairGo.transform.Find("LineCentre").GetComponent<RectTransform>();

        circle = canvas.transform.Find("Circle");
        circleScript = circle.GetComponent<ProudLlama.CircleGenerator.StrokeCircleGenerator>();

        sliderCentreGap.value = centreGap;
        sliderHorizontalLength.value = xLineLength;
        sliderHorizontalThickness.value = xLineThickness;
        sliderVerticalLength.value = yLineLength;
        sliderVeritcalThickness.value = yLineThickness;

        sliderR.value = color.r;
        sliderG.value = color.g;
        sliderB.value = color.b;

        toggleCircle.isOn = enableCircle;
        toggleLines.isOn = enableLines;
        toggleCentreDot.isOn = enableCentre;
        toggleDynamic.isOn = dynamic;


        canvas.worldCamera = this.GetComponentInParent<FPS>().camera;
        canvas.planeDistance = 0.09f;


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            menuActive = !menuActive;
        //menuActive = true;
        menuParent.SetActive(menuActive);
        if(menuActive)
        {
            doUpdate = true;
            centreGap = sliderCentreGap.value;
            xLineLength = sliderHorizontalLength.value;
            xLineThickness = sliderHorizontalThickness.value;
            yLineLength = sliderVerticalLength.value;
            yLineThickness = sliderVeritcalThickness.value;

            color.r = sliderR.value;
            color.g = sliderG.value;
            color.b = sliderB.value;

            enableCircle = toggleCircle.isOn;
            enableCentre = toggleCentreDot.isOn;
            enableLines = toggleLines.isOn;
            dynamic = toggleDynamic.isOn;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (!doUpdate)
            return;
        doUpdate = false;

        float xOffset = centreGap + xLineLength / 2 + circleThickness - 0.1f;
        float yOffset = centreGap + yLineLength / 2 + circleThickness - 0.1f;

        lineXP.sizeDelta = new Vector2(xLineLength, xLineThickness);
        lineXP.anchoredPosition = new Vector3(xOffset, 0);
        lineXP.GetComponent<Image>().color = color;

        lineXN.sizeDelta = new Vector2(xLineLength, xLineThickness);
        lineXN.anchoredPosition = new Vector3(-xOffset, 0);
        lineXN.GetComponent<Image>().color = color;

        lineYP.sizeDelta = new Vector2(yLineThickness, yLineLength);
        lineYP.anchoredPosition = new Vector3(0, yOffset);
        lineYP.GetComponent<Image>().color = color;

        lineYN.sizeDelta = new Vector2(yLineThickness, yLineLength);
        lineYN.anchoredPosition = new Vector3(0, -yOffset);
        lineYN.GetComponent<Image>().color = color;

        lineCentre.sizeDelta = new Vector2(centreThickness, centreThickness);
        lineCentre.GetComponent<Image>().color = color;

        circleScript.StrokeData = new ProudLlama.CircleGenerator.StrokeData(circleThickness, false);
        circleScript.CircleData = new ProudLlama.CircleGenerator.CircleData(centreGap, 360, 0, 64, true);
        circleScript.Generate();
        circle.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", color);

        lineXP.gameObject.SetActive(enableLines);
        lineXN.gameObject.SetActive(enableLines);
        lineYP.gameObject.SetActive(enableLines);
        lineYN.gameObject.SetActive(enableLines);

        circle.gameObject.SetActive(enableCircle);

        lineCentre.gameObject.SetActive(enableCentre);
    }

    public void UpdateBloom(float bloom, float bloomMax)
    {
        if (!dynamic)
            return;

        float bloomAmount = centreGap * bloom / bloomMax;
        float xOffset = centreGap + xLineLength / 2 + circleThickness - 0.1f + bloomAmount;
        float yOffset = centreGap + yLineLength / 2 + circleThickness - 0.1f + bloomAmount;

        lineXP.anchoredPosition = new Vector3(xOffset, 0);
        lineXN.anchoredPosition = new Vector3(-xOffset, 0);
        lineYP.anchoredPosition = new Vector3(0, yOffset);
        lineYN.anchoredPosition = new Vector3(0, -yOffset);
        circleScript.CircleData = new ProudLlama.CircleGenerator.CircleData(centreGap + bloomAmount, 360, 0, 64, true);
        circleScript.Generate();


    }
}
